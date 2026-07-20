using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Channels;
using System.Reactive.Disposables;
namespace InterReact;

// Connection instance is added to the container.
// Write to the socket using a channel.
// Read from the socket using a pipeline.
public sealed class Connection : IAsyncDisposable
{
    internal static readonly Connection NullInstance =
        new(new(SocketType.Stream, ProtocolType.Tcp), new InterReactOptions(), NullLogger.Instance, default);
    private readonly ILogger _logger;
    private readonly InterReactOptions _options;
    private readonly Channel<byte[]> _outgoing = Channel.CreateBounded<byte[]>
        (new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
    private readonly Socket _socket;
    private readonly CancellationTokenSource _cts;
    private readonly CancellationToken _ct;
    private readonly Task _senderTask;
    private Task? _receiverTask;
    private int _disposed;
    internal IPEndPoint RemoteEndPoint { get; }
    internal IObservable<string[]> Observable { get; }

    internal Connection(Socket socket, InterReactOptions options, ILogger logger, CancellationToken ct)
    {
        _socket = socket;
        _options = options;
        _logger = logger;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _ct = _cts.Token;
        _senderTask = SenderLoopAsync();
        Observable = CreateObservable();
        RemoteEndPoint = (IPEndPoint)socket.RemoteEndPoint!;
    }

    internal ValueTask SendStringAsync(string str) =>
        SendAsync(str.ToByteArray());

    // V100Plus format: payload of null-terminated strings with 4 byte message length prefix.
    internal ValueTask SendMessageAsync(IEnumerable<string> strings) =>
        SendAsync(strings.ToByteArrayWithLengthPrefix());

    // Write to the channel.
    private async ValueTask SendAsync(byte[] bytes)
    {
        try
        {
            await _outgoing.Writer.WriteAsync(bytes, _ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send() error.");
            throw;
        }
    }

    // Read from channel, write to socket.
    private async Task SenderLoopAsync()
    {
        try
        {
            await foreach (byte[] message in _outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
                await SendAllAsync(message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (_ct.IsCancellationRequested)
                return;
            _logger.LogError(ex, "SenderLoopAsync() error.");
            throw;
        }
    }

    private async ValueTask SendAllAsync(ReadOnlyMemory<byte> buffer)
    {
        while (!buffer.IsEmpty)
        {
            int sent = await _socket.SendAsync(buffer, SocketFlags.None, _ct).ConfigureAwait(false);
            if (sent == 0)
                throw new IOException("Socket closed.");
            buffer = buffer.Slice(sent);
        }
    }

    private async Task LoginAsync()
    {
        // Send without prefix
        await SendStringAsync("API").ConfigureAwait(false);

        // Send with prefix. Report a range of supported API versions to TWS.
        string s = $"v{(int)_options.ServerVersionMin}..{(int)_options.ServerVersionMax}";
        await SendMessageAsync([s]).ConfigureAwait(false);

        await SendMessageAsync([
                ((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
                "2",
                _options.TwsClientId.ToString(CultureInfo.InvariantCulture),
                _options.OptionalCapabilities
            ]).ConfigureAwait(false);

        string[] firstMessage = await ReadOneMessageAsync().ConfigureAwait(false);
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

    internal async Task<string[]> ReadOneMessageAsync()
    {
        _cts.CancelAfter(TimeSpan.FromSeconds(3));
        try
        {
            return await _socket.ReadOneMessageAsync(_ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (_ct.IsCancellationRequested)
        {
            throw new TimeoutException("Timeout waiting for response from TWS/Gateway. Try restarting.", e);
        }
    }

    internal IObservable<string[]> CreateObservable()
    {
        return System.Reactive.Linq.Observable.Create<string[]>(observer =>
        {
            _logger.LogInformation("Startup.");

            if (_receiverTask != null)
                throw new InvalidOperationException("Observable already subscribed.");
            _receiverTask = _socket.RunAsync(observer, _logger, _ct);

            return Disposable.Create(() => _cts.Cancel());
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        await _cts.CancelAsync().ConfigureAwait(false);

        _outgoing.Writer.TryComplete();

        await Task.WhenAll(_senderTask, _receiverTask ?? Task.CompletedTask).ConfigureAwait(false);

        try
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch { }

        _socket.Dispose();
        _cts.Dispose();
        _logger.LogInformation("Dispose.");
    }
  
    internal static async Task<Connection> CreateAsync(InterReactOptions options, CancellationToken ct)
    {
        ILogger logger = options.LogFactory.CreateLogger<Connection>();
        AssemblyName name = Assembly.GetExecutingAssembly().GetName();
        logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        Socket socket = await ConnectSocketAsync(options.TwsIpAddress, options.TwsPortAddresses, logger, ct).ConfigureAwait(false);
        try
        {
            Connection connection = new(socket, options, logger, ct);
            await connection.LoginAsync().ConfigureAwait(false);
            return connection;
        }
        catch (Exception)
        {
            socket.Dispose();
            throw;
        }
    }

    private static async Task<Socket> ConnectSocketAsync(IPAddress ipAddress, IReadOnlyList<int> ports, ILogger logger, CancellationToken ct)
    {
        if (ports.Count == 0)
            throw new ArgumentException("No ports specified.", nameof(ports));

        // don't reuse the Socket after a failed connect attempt
        foreach (int port in ports)
        {
            Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            try
            {
                await socket.ConnectAsync(ipAddress, port, cts.Token).ConfigureAwait(false);
                logger.LogInformation("Connected to TWS/Gateway at {IpAddress}:{Port}.", ipAddress, port);
                return socket;
                // ct cancel  => OperationCanceledException
                // cts timeout => OperationCanceledException
                // socket timeout/error => SocketException
            }
            catch (Exception)
            {
                socket.Dispose();
                if (ct.IsCancellationRequested)
                    throw;
                // timeout -> try next port
            }
        }
        string message = $"Could not connect to TWS/Gateway at " +
            $"[{ipAddress}]:{string.Join(", ", ports)}. " +
            $"In TWS, navigate to Edit / Global Configuration / API / Settings and ensure" +
            $" the option \"Enable ActiveX and Socket Clients\" is selected.";
        logger.LogCritical("{Message}",  message);
        throw new InvalidOperationException(message);
    }

}
