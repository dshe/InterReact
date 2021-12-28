using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Stringification;
using RxSockets;
using System.Reflection;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace InterReact;

public sealed class InterReactClientBuilder
{
    public static InterReactClientBuilder Create() => new();
    private InterReactClientBuilder() { }

    internal IClock Clock { get; private set; } = SystemClock.Instance;
    internal InterReactClientBuilder WithClock(IClock clock)
    {
        Clock = clock;
        return this;
    }

    internal ILogger Logger { get; private set; } = NullLogger.Instance;
    internal bool LogIncomingMessages { get; private set; }
    public InterReactClientBuilder WithLogger(ILogger logger, bool logIncomingMessages = false)
    {
        Logger = logger;
        LogIncomingMessages = logIncomingMessages;
        return this;
    }

    public IPEndPoint IPEndPoint { get; } = new(IPAddress.IPv6Loopback, 0);
    public InterReactClientBuilder WithIpAddress(IPAddress address)
    {
        IPEndPoint.Address = address;
        return this;
    }
    public bool IsDemoAccount => IPEndPoint.Port == (int)DefaultPort.TwsDemoAccount || IPEndPoint.Port == (int)DefaultPort.GatewayDemoAccount;

    internal IReadOnlyList<int> Ports { get; private set; } =
        new[] { (int)DefaultPort.TwsRegularAccount, (int)DefaultPort.TwsDemoAccount, (int)DefaultPort.GatewayRegularAccount, (int)DefaultPort.GatewayDemoAccount };
    /// <summary>
    /// Specify a port to attempt connection to TWS/Gateway.
    /// Otherwise, connection will be attempted on ports 7496 and 7497, 4001, 4002.
    /// </summary>
    public InterReactClientBuilder WithPorts(params int[] ports)
    {
        Ports = ports;
        return this;
    }

    public int ClientId { get; private set; } = RandomNumberGenerator.GetInt32(100000, 999999);
    /// <summary>
    /// Up to 8 clients can attach to TWS/Gateway. Each client requires a unique Id. The default Id is random.
    /// </summary>
    public InterReactClientBuilder WithClientId(int id)
    {
        ClientId = id >= 0 ? id : throw new ArgumentException("invalid", nameof(id));
        return this;
    }


    internal int MaxRequestsPerSecond { get; private set; } = 50;
    /// <summary>
    /// Indicate the maximum number of requests per second sent to to TWS/Gateway.
    /// </summary>
    public InterReactClientBuilder WithMaxRequestsPerSecond(int requests)
    {
        MaxRequestsPerSecond = requests > 0 ? requests : throw new ArgumentException("invalid", nameof(requests));
        return this;
    }

    internal string OptionalCapabilities { get; private set; } = "";
    public InterReactClientBuilder WithOptionalCapabilities(string capabilities)
    {
        OptionalCapabilities = capabilities;
        return this;
    }

    internal bool FollowPriceTickWithSizeTick { get; private set; }
    public InterReactClientBuilder WithFollowPriceTickWithSizeTick()
    {
        FollowPriceTickWithSizeTick = true;
        return this;
    }

    public const ServerVersion ServerVersionMin = ServerVersion.FRACTIONAL_POSITIONS;
    public ServerVersion ServerVersionMax { get; private set; } = ServerVersion.WSHE_CALENDAR;
    public InterReactClientBuilder WithMaxServerVersion(ServerVersion maxServerVersion)
    {
        ServerVersionMax = maxServerVersion;
        return this;
    }
    public ServerVersion ServerVersionCurrent { get; private set; } = ServerVersion.NONE;
    internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;

    public string Date { get; private set; } = "";

    /////////////////////////////////////////////////////////////

    public async Task<IInterReactClient> BuildAsync(CancellationToken ct = default)
    {
        AssemblyName name = GetType().Assembly.GetName();
        Logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        IRxSocketClient? rxSocket = null;
        try
        {
            rxSocket = await Initialize(ct).ConfigureAwait(false);

            Stringifier stringifier = new(Logger);

            IObservable<object> response = rxSocket
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .ToObservableFromAsyncEnumerable()
                .ToMessages(this)
                .Do(m =>
                {
                    if (LogIncomingMessages)
                        Logger.LogInformation("Incoming message: {Message}", stringifier.Stringify(m));
                })
                .Publish()
                .AutoConnect(); // connect on first observer

            return new ServiceCollection()
                .AddSingleton(this) // builder
                .AddSingleton(stringifier)
                .AddSingleton(rxSocket) // instance of RxSocketClient
                .AddSingleton(response) // IObservable<object>
                .AddSingleton<Request>()
                .AddSingleton<Services>()
                .AddSingleton<IInterReactClient, InterReactClient>()
                .BuildServiceProvider()
                .GetService<IInterReactClient>() ?? throw new InvalidOperationException("GetService<IInterReactClient>.");
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "InterReactClientBuilder");
            if (rxSocket != null)
                await rxSocket.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private async Task<IRxSocketClient> Initialize(CancellationToken ct)
    {
        var rxsocket = await ConnectAsync(ct).ConfigureAwait(false);
        await Login(rxsocket, ct).ConfigureAwait(false);
        Logger.LogInformation("Logged into Tws/Gateway using clientId: {ClientId} and server version: {ServerVersionCurrent}.", ClientId, (int)ServerVersionCurrent);
        return rxsocket;
    }

    private async Task<IRxSocketClient> ConnectAsync(CancellationToken ct)
    {
        foreach (int port in Ports)
        {
            IPEndPoint.Port = port;
            try
            {
                return await IPEndPoint.CreateRxSocketClientAsync(Logger, ct).ConfigureAwait(false);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
            {
                Logger.LogTrace("{Message}", e.Message);
            }
        }
        string ports = Ports.Select(p => p.ToString(CultureInfo.InvariantCulture)).JoinStrings(", ");
        string message = $"Could not connect to TWS/Gateway at [{IPEndPoint.Address}]:{ports}.";
        throw new ArgumentException(message);
    }

    private async Task Login(IRxSocketClient rxsocket, CancellationToken ct)
    {
        Send("API");

        // Report a range of supported API versions to TWS.
        SendWithPrefix($"v{(int)ServerVersionMin}..{(int)ServerVersionMax}");

        SendWithPrefix(((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
            "2", ClientId.ToString(CultureInfo.InvariantCulture), OptionalCapabilities);

        string[] message = await GetMessage().ConfigureAwait(false);

        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(message[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
        ServerVersionCurrent = version;
        Date = message[1];

        // local methods
        void Send(string str) => rxsocket
            .Send(str.ToByteArray());

        void SendWithPrefix(params string[] strings) => rxsocket
            .Send(strings.ToByteArray().ToByteArrayWithLengthPrefix());

        async Task<string[]> GetMessage() => await rxsocket
            .ReceiveAllAsync()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .FirstAsync(ct)
            .ConfigureAwait(false);
    }
}
