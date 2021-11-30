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
namespace InterReact;

public sealed class InterReactClientBuilder
{
    public static InterReactClientBuilder Create() => new();
    private InterReactClientBuilder() { }

    private readonly Config Config = new();
    private ILogger Logger => Config.Logger;

    internal InterReactClientBuilder WithClock(IClock clock)
    {
        Config.Clock = clock;
        return this;
    }

    public InterReactClientBuilder WithLogger(ILogger logger, bool logIncomingMessages = false)
    {
        Config.Logger = logger;
        Config.LogIncomingMessages = logIncomingMessages;
        return this;
    }

    /// <summary>
    /// Specify an IPAddress to connect to TWS/Gateway.
    /// </summary>
    public InterReactClientBuilder WithMaxServerVersion(ServerVersion maxServerVersion)
    {
        Config.ServerVersionMax = maxServerVersion;
        return this;
    }

    /// <summary>
    /// Specify an IPAddress to connect to TWS/Gateway.
    /// </summary>
    public InterReactClientBuilder WithIpAddress(IPAddress address)
    {
        Config.IPEndPoint.Address = address;
        return this;
    }

    /// <summary>
    /// Specify a port to attempt connection to TWS/Gateway.
    /// Otherwise, connection will be attempted on ports 7496 and 7497, 4001, 4002.
    /// </summary>
    public InterReactClientBuilder WithPort(int port)
    {
        Config.Ports = new[] { port };
        return this;
    }

    /// <summary>
    /// Up to 8 clients can attach to TWS/Gateway. Each client requires a unique Id. The default Id is random.
    /// Only ClientId = 0 is able to modify orders submitted manually through TWS.
    /// </summary>
    public InterReactClientBuilder WithClientId(int id)
    {
        Config.ClientId = id >= 0 ? id : throw new ArgumentException("invalid", nameof(id));
        return this;
    }

    /// <summary>
    /// Indicate the maximum number of requests per second sent to to TWS/Gateway.
    /// </summary>
    public InterReactClientBuilder WithMaxRequestsPerSecond(int requests)
    {
        Config.MaxRequestsPerSecond = requests > 0 ? requests : throw new ArgumentException("invalid", nameof(requests));
        return this;
    }

    public InterReactClientBuilder WithOptionalCapabilities(string capabilities)
    {
        Config.OptionalCapabilities = capabilities;
        return this;
    }

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
                .ToMessages(Config)
                .Do(m =>
                {
                    if (Config.LogIncomingMessages)
                        Logger.LogInformation("Incoming message: {Message}", stringifier.Stringify(m));
                })
                .Publish()
                .AutoConnect(); // connect on first observer

            return new ServiceCollection()
                .AddSingleton(stringifier)
                .AddSingleton(Config)   // Config has no dependencies
                .AddSingleton(rxSocket) // rxSocket is an instance of RxSocketClient
                .AddSingleton(response) // response is IObservable<object>
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
        Logger.LogInformation("Logged into Tws/Gateway using clientId: {ClientId} and server version: {ServerVersionCurrent}.", Config.ClientId, (int)Config.ServerVersionCurrent);
        return rxsocket;
    }

    private async Task<IRxSocketClient> ConnectAsync(CancellationToken ct)
    {
        foreach (int port in Config.Ports)
        {
            Config.IPEndPoint.Port = port;
            try
            {
                return await Config.IPEndPoint.CreateRxSocketClientAsync(Logger, ct).ConfigureAwait(false);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
            {
                Logger.LogTrace("{Message}", e.Message);
            }
        }
        string ports = Config.Ports.Select(p => p.ToString(CultureInfo.InvariantCulture)).JoinStrings(", ");
        string message = $"Could not connect to TWS/Gateway at [{Config.IPEndPoint.Address}]:{ports}.";
        throw new ArgumentException(message);
    }

    private async Task Login(IRxSocketClient rxsocket, CancellationToken ct)
    {
        Send("API");

        // Report a range of supported API versions to TWS.
        SendWithPrefix($"v{(int)Config.ServerVersionMin}..{(int)Config.ServerVersionMax}");

        SendWithPrefix(((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
            "2", Config.ClientId.ToString(CultureInfo.InvariantCulture), Config.OptionalCapabilities);

        string[] message = await GetMessage().ConfigureAwait(false);

        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(message[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
        Config.ServerVersionCurrent = version;
        Config.Date = message[1];

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
