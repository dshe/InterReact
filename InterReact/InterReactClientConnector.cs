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

public sealed record InterReactClientConnector
{
    internal IClock Clock { get; private set; } = SystemClock.Instance;
    internal InterReactClientConnector WithClock(IClock clock) => this with { Clock = clock };

    internal ILogger Logger { get; private set; } = NullLogger.Instance;
    public InterReactClientConnector WithLogger(ILogger logger) => this with { Logger = logger };

    public IPAddress IPAddress { get; private set; } = IPAddress.IPv6Loopback;
    public InterReactClientConnector WithIpAddress(IPAddress address) => this with { IPAddress = address };

    internal IReadOnlyList<int> Ports { get; private set; } =
        new[] { (int)IBDefaultPort.TwsRegularAccount, (int)IBDefaultPort.TwsDemoAccount, (int)IBDefaultPort.GatewayRegularAccount, (int)IBDefaultPort.GatewayDemoAccount };
    /// <summary>
    /// Specify the port used to attempt connection to TWS/Gateway.
    /// If unspecified, connection will be attempted on ports 7496 and 7497, 4001, 4002.
    /// </summary>
    public InterReactClientConnector WithPort(int port) => this with { Ports = new[] { port } };

    public int ClientId { get; private set; } = RandomNumberGenerator.GetInt32(100000, 999999);
    /// <summary>
    /// Specify a client id.
    /// Up to 8 clients can attach to TWS/Gateway. Each client requires a unique Id. The default Id is random.
    /// </summary>
    public InterReactClientConnector WithClientId(int id) => 
        this with { ClientId = id >= 0 ? id : throw new ArgumentException("Invalid ClientId", nameof(id)) };

    internal int MaxRequestsPerSecond { get; private set; } = 50;
    /// <summary>
    /// Specify the maximum number of requests per second sent to to TWS/Gateway.
    /// </summary>
    public InterReactClientConnector WithMaxRequestsPerSecond(int requests) =>
        this with { MaxRequestsPerSecond = requests > 0 ? requests : throw new ArgumentException("invalid", nameof(requests)) };

    internal string OptionalCapabilities { get; private set; } = "";
    public InterReactClientConnector WithOptionalCapabilities(string capabilities) => this with { OptionalCapabilities = capabilities };

    internal bool FollowPriceTickWithSizeTick { get; private set; }
    public InterReactClientConnector WithFollowPriceTickWithSizeTick() => this with { FollowPriceTickWithSizeTick = true };


    public const ServerVersion ServerVersionMin = ServerVersion.FRACTIONAL_POSITIONS;
    public ServerVersion ServerVersionMax { get; private set; } = ServerVersion.WSHE_CALENDAR;
    public InterReactClientConnector WithMaxServerVersion(ServerVersion maxServerVersion) => this with { ServerVersionMax = maxServerVersion };

    public ServerVersion ServerVersionCurrent { get; private set; } = ServerVersion.NONE;
    internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;

    public string Date { get; private set; } = "";

    /////////////////////////////////////////////////////////////

    public async Task<IInterReactClient> ConnectAsync(CancellationToken ct = default)
    {
        AssemblyName name = GetType().Assembly.GetName();
        Logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        IRxSocketClient? rxSocket = null;
        try
        {
            rxSocket = await ConnectSocketAsync(ct).ConfigureAwait(false);
            await Login(rxSocket, ct).ConfigureAwait(false);
            Logger.LogInformation("Logged into Tws/Gateway using clientId: {ClientId} and server version: {ServerVersionCurrent}.", ClientId, (int)ServerVersionCurrent);

            Stringifier stringifier = new(Logger);

            IObservable<object> response = rxSocket
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .ToObservableFromAsyncEnumerable()
                .ToMessages(this)
                .Do(message =>
                {
                    if (Logger.IsEnabled(LogLevel.Debug))
                        Logger.LogDebug("Incoming message: {Message}", stringifier.Stringify(message));
                })
                .Publish()
                .AutoConnect(); // connect on first observer

            return new ServiceCollection()
                .AddSingleton(this) // connector
                .AddSingleton(stringifier)
                .AddSingleton(rxSocket) // instance of RxSocketClient
                .AddSingleton(response) // IObservable<object>
                .AddSingleton<Request>()
                .AddSingleton<Service>()
                .AddSingleton<IInterReactClient, InterReactClient>()
                .BuildServiceProvider()
                .GetService<IInterReactClient>() ?? throw new InvalidOperationException("GetService<IInterReactClient>.");
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "InterReactClientConnector");
            if (rxSocket is not null)
                await rxSocket.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private async Task<IRxSocketClient> ConnectSocketAsync(CancellationToken ct)
    {
        IPEndPoint ipEndPoint = new(IPAddress, 0);
        foreach (int port in Ports)
        {
            ipEndPoint.Port = port;
            try
            {
                return await ipEndPoint.CreateRxSocketClientAsync(Logger, ct).ConfigureAwait(false);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
            {
                Logger.LogTrace("{Message}", e.Message);
            }
        }
        string ports = Ports.Select(p => p.ToString(CultureInfo.InvariantCulture)).JoinStrings(", ");
        string message = $"Could not connect to TWS/Gateway at [{ipEndPoint.Address}]:{ports}. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option \"Enable ActiveX and Socket Clients\" is selected.";
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
