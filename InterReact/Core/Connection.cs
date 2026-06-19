using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Channels;
namespace InterReact;

public sealed class Connection : IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly InterReactOptions _options;
    private readonly Socket _socket;
    private readonly Task _senderTask;
    private readonly Channel<byte[]> _outgoing = Channel.CreateBounded<byte[]>
        (new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
    private int _disposed;
    internal IPEndPoint RemoteEndPoint { get; }
    internal IObservable<string[]> Observable { get; }

    private Connection(Socket socket, InterReactOptions options, ILogger logger)
    {
        _socket = socket;
        _options = options;
        _logger = logger;
        _senderTask = SenderLoopAsync();
        Observable = _socket.CreateObservable().Publish().AutoConnect();
        RemoteEndPoint = (IPEndPoint)socket.RemoteEndPoint!;
    }

    internal async Task<string[]> ReadOneMessageAsync(CancellationToken ct)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        try
        {
            return await _socket.ReadOneMessageAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException("Timeout waiting for response from TWS/Gateway. Try restarting.", e);
        }
    }

    internal ValueTask SendStringAsync(string str, CancellationToken ct = default)
    {
        byte[] message = str.ToByteArray();
        return _outgoing.Writer.WriteAsync(message, ct);
    }

    // V100Plus format: payload of null-terminated strings with 4 byte message length prefix.
    internal ValueTask SendMessageAsync(IEnumerable<string> strings, CancellationToken ct = default)
    {
        byte[] message = strings.ToByteArrayWithLengthPrefix();
        return _outgoing.Writer.WriteAsync(message, ct);
    }

    private async Task SenderLoopAsync()
    {
        await foreach (byte[] message in _outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
            await SendAllAsync(message).ConfigureAwait(false);
    }

    private async ValueTask SendAllAsync(ReadOnlyMemory<byte> buffer)
    {
        while (!buffer.IsEmpty)
        {
            int sent = await _socket.SendAsync(buffer, SocketFlags.None).ConfigureAwait(false);
            if (sent == 0)
                throw new IOException("Socket closed.");
            buffer = buffer.Slice(sent);
        }
    }

    private async Task LoginAsync(CancellationToken ct)
    {
        // Send without prefix
        await SendStringAsync("API", ct).ConfigureAwait(false);

        // Send with prefix. Report a range of supported API versions to TWS.
        string s = $"v{(int)_options.ServerVersionMin}..{(int)_options.ServerVersionMax}";
        await SendMessageAsync([s], ct).ConfigureAwait(false);

        await SendMessageAsync([
                ((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
                "2",
                _options.TwsClientId.ToString(CultureInfo.InvariantCulture),
                _options.OptionalCapabilities
            ], ct).ConfigureAwait(false);

        string[] firstMessage = await ReadOneMessageAsync(ct).ConfigureAwait(false);
        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(firstMessage[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{firstMessage[0]}'.");
        _options.ServerVersionCurrent = version;
        if (_options.ServerVersionCurrent < _options.ServerVersionMin || _options.ServerVersionCurrent > _options.ServerVersionMax)
            throw new InvalidDataException($"Invalid server version '{_options.ServerVersionCurrent}'.");
        // The rest of the message is ignored and contains the date, time and timezone.

        _logger.LogInformation("Logged into TWS/Gateway version {ServerVersionCurrent} with ClientId {ClientId}.",
            (int)_options.ServerVersionCurrent, _options.TwsClientId);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;
        _outgoing.Writer.TryComplete();
        try
        {
            await _senderTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            try
            {
                _socket?.Shutdown(SocketShutdown.Send);
            } 
            catch { }
            _socket?.Dispose();
        }
    }
  
    internal static async Task<Connection> CreateAsync(InterReactOptions options, CancellationToken ct)
    {
        ILogger logger = options.LogFactory.CreateLogger<Connection>();
        AssemblyName name = Assembly.GetExecutingAssembly().GetName();
        logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        Socket socket = await ConnectSocketAsync(options.TwsIpAddress, options.TwsPortAddresses, logger, ct).ConfigureAwait(false);
        Connection connection = new(socket, options, logger);
        await connection.LoginAsync(ct).ConfigureAwait(false);
        return connection;
    }

    private static async Task<Socket> ConnectSocketAsync(IPAddress ipAddress, IReadOnlyList<int> ports, ILogger logger, CancellationToken ct)
    {
        TimeSpan connectTimeout = TimeSpan.FromSeconds(3);
        if (ports.Count == 0)
            throw new ArgumentException("No ports specified.", nameof(ports));

        // don't reuse the Socket after a failed connect attempt
        foreach (int port in ports)
        {
            Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(connectTimeout);
            try
            {
                await socket.ConnectAsync(ipAddress, port, cts.Token).ConfigureAwait(false);
                logger.LogInformation("Connected to TWS/Gateway at {IpAddress}:{Port}.", ipAddress, port);
                return socket;
                // token cancel  => OperationCanceledException
                // token timeout => OperationCanceledException
                // socket timeout/error => SocketException
            }
            catch (OperationCanceledException)
            {
                socket.Dispose();
                if (ct.IsCancellationRequested)
                    throw;
                // timeout -> try next port
            }
            catch (SocketException)
            {
                socket.Dispose();
                // connection failed -> try next port
            }
        }
        string message = $"Could not connect to TWS/Gateway at " +
            $"[{ipAddress}]:{string.Join(", ", ports)}. " +
            $"In TWS, navigate to Edit / Global Configuration / API / Settings and ensure" +
            $" the option \"Enable ActiveX and Socket Clients\" is selected.";
        logger.LogCritical("{Message}",  message);
        throw new InvalidOperationException(message);
    }

    // for testing
    internal static Connection NullInstance => new(new(SocketType.Stream, ProtocolType.Tcp), new InterReactOptions(null), NullLogger.Instance);
}
