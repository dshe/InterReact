using Microsoft.Extensions.DependencyInjection;
using RxSockets;
using Stringification;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Reflection;

namespace InterReact;

public sealed class Connection : IAsyncDisposable
{
    private readonly ILoggerFactory LogFactory;
    private readonly ILogger Logger;
    private IRxSocketClient RxSocketClient = NullRxSocketClient.Instance;
    internal IPEndPoint RemoteIpEndPoint => (IPEndPoint) RxSocketClient.RemoteEndPoint;
    internal IClock Clock { get; }
    internal IPAddress IpAddress { get; }
    internal IReadOnlyList<int> Ports { get; }
    internal int ClientId { get; }
    private int MaxRequestsPerSecond { get; }
    internal string OptionalCapabilities { get; }
    internal bool FollowPriceTickWithSize { get; }
    internal bool UseDelayedTicks { get; }
    internal ServerVersion ServerVersionMin { get; } = InterReactClientConnector.ServerVersionMin;
    internal ServerVersion ServerVersionMax { get; }
    internal ServerVersion ServerVersionCurrent { get; private set; } = ServerVersion.NONE;
    internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;
    internal void RequireServerVersion(ServerVersion version)
    {
        if (!SupportsServerVersion(version))
            throw new ArgumentException($"The server does not support version: '{version}'.");
    }

    // Id is incremented by Request.GetNextId().
    // Id is updated in the constructor of the NextOrderId message which is received at startup.
    // NextOrderId message is also received in response to Request.RequestNextOrderId().
    internal int Id;
    
    internal Connection(InterReactClientConnector connection)
    {
        LogFactory = connection.LogFactory;
        Logger = connection.LogFactory.CreateLogger("InterReact.Connection");
        Clock = connection.Clock;
        IpAddress= connection.IpAddress;
        Ports = connection.Ports;
        ClientId = connection.ClientId;
        MaxRequestsPerSecond = connection.MaxRequestsPerSecond;
        OptionalCapabilities= connection.OptionalCapabilities;
        UseDelayedTicks = connection.UseDelayedTicks;
        ServerVersionMax= connection.ServerVersionMax;
    }

    internal async Task<IInterReactClient> ConnectAsync(CancellationToken ct = default)
    {
        AssemblyName name = GetType().Assembly.GetName();
        Logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        await ConnectSocketAsync(ct).ConfigureAwait(false);
        await Login(ct).ConfigureAwait(false);
        Logger.LogInformation("Logged into Tws/Gateway using clientId: {ClientId} and server version: {ServerVersionCurrent}.",
            ClientId, (int)ServerVersionCurrent);

        return new ServiceCollection()
            .AddSingleton(this) // Connection
            .AddSingleton(LogFactory)
            .AddSingleton(RxSocketClient) // instance
            .AddSingleton(new RingLimiter(MaxRequestsPerSecond))
            .AddSingleton<Stringifier>()
            .AddSingleton<Request>()
            .AddTransient<RequestMessage>() // transient!
            .AddSingleton<Func<RequestMessage>>(s => () => s.GetRequiredService<RequestMessage>())
            .AddSingleton<Response>() // IObservable<object>
            .AddSingleton<Service>()
            .AddSingleton<IInterReactClient, InterReactClient>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true })
            .GetRequiredService<IInterReactClient>();
    }

    private async Task ConnectSocketAsync(CancellationToken ct)
    {
        IPEndPoint ipEndPoint = new(IpAddress, 0);
        foreach (int port in Ports)
        {
            ipEndPoint.Port = port;
            try
            {
                ILogger rxSocketLogger = LogFactory.CreateLogger("InterReact.RxSocketClient");
                RxSocketClient = await ipEndPoint.CreateRxSocketClientAsync(rxSocketLogger, ct).ConfigureAwait(false);
                return;
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
            {
            }
        }
        string ports = Ports.Select(p => p.ToString(CultureInfo.InvariantCulture)).JoinStrings(", ");
        string message = $"Could not connect to TWS/Gateway at [{ipEndPoint.Address}]:{ports}. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option \"Enable ActiveX and Socket Clients\" is selected.";
        throw new ArgumentException(message);
    }

    private async Task Login(CancellationToken ct)
    {
        // send without prefix
        RxSocketClient.Send("API".ToByteArray());

        // Report a range of supported API versions to TWS.
        SendWithPrefix($"v{(int)ServerVersionMin}..{(int)ServerVersionMax}");

        SendWithPrefix(((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
            "2", ClientId.ToString(CultureInfo.InvariantCulture), OptionalCapabilities);

        string[] message;
        try
        {
            message = await RxSocketClient
                .ReceiveAllAsync
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync(ct)
                .AsTask()
                .WaitAsync(TimeSpan.FromSeconds(3), ct)
                .ConfigureAwait(false);
        }
        catch (TimeoutException e)
        {
            throw new TimeoutException("Timeout waiting for the first response from TWS/Gateway. Try restarting.", e);
        }

        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(message[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
        ServerVersionCurrent = version;
        // message[1] is Date

        // local method
        void SendWithPrefix(params string[] strings) => RxSocketClient
            .Send(strings.ToByteArray().ToByteArrayWithLengthPrefix());
    }

    public async ValueTask DisposeAsync() =>
        await RxSocketClient.DisposeAsync().ConfigureAwait(false);
}
