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
using InterReact.Utility;

namespace InterReact
{
    public sealed class InterReactClientBuilder : EditorBrowsableNever
    {
        private readonly ILogger Logger;
        private readonly Config Config = new();

        public InterReactClientBuilder() : this(NullLogger.Instance) { }
        public InterReactClientBuilder(ILogger<InterReactClient> logger) : this((ILogger)logger) { }
        public InterReactClientBuilder(ILogger logger) // supports testing
        {
            Logger = logger;
            var name = GetType().Assembly.GetName();
            Logger.LogTrace($"{name.Name} version {name.Version}.");
        }

        /// <summary>
        /// Specify an IPAddress to connect to TWS/Gateway.
        /// </summary>
        public InterReactClientBuilder SetIpAddress(IPAddress address)
        {
            Config.IPEndPoint.Address = address;
            return this;
        }

        /// <summary>
        /// Specify one or more ports to attempt connection to TWS/Gateway.
        /// Otherwise, connection will be attempted on ports 7496 and 7497, 4001, 4002.
        /// </summary>
        public InterReactClientBuilder SetPort(params int[] ports)
        {
            Config.Ports = (ports != null && ports.Any()) ? ports : throw new ArgumentNullException(nameof(ports));
            return this;
        }

        /// <summary>
        /// Up to 8 clients can attach to TWS/Gateway. Each client requires a unique Id. The default Id is random.
        /// Only ClientId = 0 is able to modify orders submitted manually through TWS.
        /// </summary>
        public InterReactClientBuilder SetClientId(int id)
        {
            Config.ClientId = id >= 0 ? id : throw new ArgumentException("invalid", nameof(id));
            return this;
        }

        /// <summary>
        /// Indicate the maximum number of requests per second sent to to TWS/Gateway.
        /// </summary>
        public InterReactClientBuilder SetMaxRequestsPerSecond(int requests)
        {
            Config.MaxRequestsPerSecond = requests > 0 ? requests : throw new ArgumentException("invalid", nameof(requests));
            return this;
        }

        public InterReactClientBuilder SetOptionalCapabilities(string capabilities)
        {
            Config.OptionalCapabilities = capabilities;
            return this;
        }

        public InterReactClientBuilder SetClock(IClock clock)
        {
            Config.Clock = clock;
            return this;
        }

        /////////////////////////////////////////////////////////////

        public async Task<IInterReactClient> BuildAsync(CancellationToken ct = default)
        {
            var rxSocket = await ConnectAsync(Config, Logger, ct).ConfigureAwait(false);

            try
            {
                await Login(rxSocket, Config, Logger, ct).ConfigureAwait(false);

                var response = rxSocket
                    .ReceiveAllAsync()
                    .ToArraysFromBytesWithLengthPrefix()
                    .ToStringArrays()
                    .ToObservableFromAsyncEnumerable()
                    .ToMessages(Config, Logger)
                    .DoIntercept(Config, Logger)
                    .Publish()
                    .AutoConnect();

                return new ServiceCollection()
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
            foreach (var port in config.Ports)
            {
                config.IPEndPoint.Port = port;
                try
                {
                    return await config.IPEndPoint.CreateRxSocketClientAsync(logger, ct).ConfigureAwait(false);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.ConnectionRefused && e.SocketErrorCode != SocketError.TimedOut)
                        throw;
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
            SendWithPrefix($"v{Config.ServerVersionMin}..{Config.ServerVersionMax}");
            SendWithPrefix(
                ((int)RequestCode.StartApi).ToString(),
                "2", 
                config.ClientId.ToString(),
                config.OptionalCapabilities);

            string[] message = await GetMessage().ConfigureAwait(false);
            if (!Enum.TryParse(message[0], out ServerVersion version))
                throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
            config.ServerVersionCurrent = version;
            logger.LogDebug($"Server Version: {config.ServerVersionCurrent}.");
            // ServerVersion is the highest supported API version in the range specified.

            config.Date = message[1];
            logger.LogDebug($"Date: {config.Date}.");

            logger.LogInformation($"Logged into Tws/Gateway using ClientId: {config.ClientId}, ServerVersion: {config.ServerVersionCurrent}.");

            void Send(string str) => rxsocket.Send(str.ToByteArray());
            void SendWithPrefix(params string[] strings) => rxsocket.Send(strings.ToByteArray().ToByteArrayWithLengthPrefix());
            async Task<string[]> GetMessage() => await rxsocket
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync(ct)
                .ConfigureAwait(false);
        }
    }

    internal static class InterReactClientBuilderExtensions
    {
        internal static IObservable<object> DoIntercept(this IObservable<object> source, Config config, ILogger logger)
        {
            return source.Do(obj =>
            {
                if (obj is NextId nextId)
                {
                    int id = nextId.Id;
                    logger.LogDebug($"NextId: {id}.");
                    config.NextIdValue = id;
                }
                else if (obj is ManagedAccounts managedAccounts)
                {
                    string accounts = managedAccounts.Accounts;
                    logger.LogDebug($"ManagedAccounts: {accounts}.");
                    config.ManagedAccounts = accounts;
                }
            });
        }
    }
}
