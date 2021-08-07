using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Stringification;
using RxSockets;
using System.Reflection;

namespace InterReact
{
    public sealed class InterReactClientBuilder : EditorBrowsableNever
    {
        public static InterReactClientBuilder Create() => new();
        private InterReactClientBuilder() { }

        private ILogger Logger = NullLogger.Instance;
        private readonly Config Config = new();

        public InterReactClientBuilder WithLogger(ILogger logger, bool logIncomingMessages = false)
        {
            Logger = logger;
            Config.LogIncomingMessages = logIncomingMessages;
            AssemblyName name = GetType().Assembly.GetName();
            Logger.LogInformation($"{name.Name} version {name.Version}.");
            return this;
        }

        /// <summary>
        /// Specify an IPAddress to connect to TWS/Gateway.
        /// </summary>
        public InterReactClientBuilder WithMaxServerVersion(ServerVersion maxServerVersion)
        {
            if (maxServerVersion > ServerVersion.POST_TO_ATS)
                throw new ArgumentException("ServerVersion");
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

        public InterReactClientBuilder WithClock(IClock clock)
        {
            Config.Clock = clock;
            return this;
        }

        /////////////////////////////////////////////////////////////

        public async Task<IInterReactClient> BuildAsync(CancellationToken ct = default)
        {
            IRxSocketClient rxSocket = await ConnectAsync(Config, Logger, ct).ConfigureAwait(false);

            try
            {
                await Login(rxSocket, Config, Logger, ct).ConfigureAwait(false);

                Stringifier stringifier = new(Logger);

                IObservable<object> response = rxSocket
                    .ReceiveAllAsync()
                    .ToArraysFromBytesWithLengthPrefix()
                    .ToStringArrays()
                    .ToObservableFromAsyncEnumerable()
                    .ToMessages(Config, stringifier, Logger)
                    .Publish()
                    .AutoConnect(); // connect on first subscription

                return new ServiceCollection()
                    .AddSingleton(stringifier)
                    .AddSingleton(Logger)
                    .AddSingleton(Config)   // Config has no dependencies
                    .AddSingleton(rxSocket) // rxSocket is an instance of RxSocketClient
                    .AddSingleton(response) // response is IObservable<object>
                    .AddSingleton(new Limiter(Config.MaxRequestsPerSecond)) // configured instance
                    .AddSingleton<Request>()
                    .AddSingleton<Services>()
                    .AddSingleton<IInterReactClient, InterReactClient>()
                    .BuildServiceProvider()
                    .GetService<IInterReactClient>() ?? throw new InvalidOperationException("service");
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "InterReactClientBuilder");
                await rxSocket.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        private static async Task<IRxSocketClient> ConnectAsync(Config config, ILogger logger, CancellationToken ct)
        {
            foreach (int port in config.Ports)
            {
                config.IPEndPoint.Port = port;
                try
                {
                    return await config.IPEndPoint.CreateRxSocketClientAsync(logger, ct).ConfigureAwait(false);
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
                {
                    ;
                }
            }
            string ports = config.Ports.Select(p => p.ToString()).JoinStrings(", ");
            string message = $"Could not connect to TWS/Gateway at [{config.IPEndPoint.Address}]:{ports}.";
            throw new ArgumentException(message);
        }

        private static async Task Login(IRxSocketClient rxsocket, Config config, ILogger logger, CancellationToken ct)
        {
            Send("API");

            // Report a range of supported API versions to TWS.
            SendWithPrefix($"v{config.ServerVersionMin}..{config.ServerVersionMax}");
            SendWithPrefix(((int)RequestCode.StartApi).ToString(),
                "2", config.ClientId.ToString(), config.OptionalCapabilities);

            string[] message = await GetMessage().ConfigureAwait(false);
            //if (!int.TryParse(message[0], out int version))
            //    throw new InvalidDataException($"Could not parse server version '{message[0]}'.");

            if (!Enum.TryParse(message[0], out ServerVersion version))
                throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
            config.ServerVersionCurrent = version;

            logger.LogDebug($"Server Version: {config.ServerVersionCurrent}.");
            // ServerVersion is the highest supported API version in the range specified.

            config.Date = message[1];
            logger.LogDebug($"Date: {config.Date}.");

            logger.LogInformation($"Logged into Tws/Gateway using ClientId: {config.ClientId}, ServerVersion: {config.ServerVersionCurrent}.");

            // local methods
            void Send(string str) => 
                rxsocket
                .Send(str.ToByteArray());
            
            void SendWithPrefix(params string[] strings) => 
                rxsocket
                .Send(strings.ToByteArray()
                .ToByteArrayWithLengthPrefix());
            
            async Task<string[]> GetMessage() => 
                await rxsocket
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync(ct)
                .ConfigureAwait(false);
        }
    }
}
