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
        private readonly Config Config = new Config();

        public InterReactClientBuilder() : this(NullLogger.Instance) { }
        public InterReactClientBuilder(ILogger<InterReactClient> logger) : this((ILogger)logger) { }
        public InterReactClientBuilder(ILogger logger) // supports testing
        {
            Logger = logger;
            var name = GetType().Assembly.GetName();
            Logger.LogInformation($"{name.Name} version {name.Version}.");
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
            Config.ClientId = id >= 0 ? id : throw new ArgumentException(nameof(id));
            return this;
        }

        /// <summary>
        /// Indicate the maximum number of requests per second sent to to TWS/Gateway.
        /// </summary>
        public InterReactClientBuilder SetMaxRequestsPerSecond(int requests)
        {
            Config.MaxRequestsPerSecond = requests > 0 ? requests : throw new ArgumentException(nameof(requests));
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
            IRxSocketClient? rxSocket = null;

            try
            {
                rxSocket = await ConnectAsync(Config, ct, Logger).ConfigureAwait(false);
                Logger.LogInformation($"Connected to server at {Config.IPEndPoint}.");

                await Login(rxSocket, Config, ct, Logger).ConfigureAwait(false);
                Logger.LogInformation($"Logged into Tws/Gateway using ClientId: {Config.ClientId}, ServerVersion: {Config.ServerVersionCurrent}.");

                var response = rxSocket
                    .ReceiveObservable
                    .RemoveLengthPrefix()
                    .ToStrings()
                    .ToMessages(Config, Logger)
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
                Logger.LogCritical(ex, "InterReactClientBuilder.BuildAsync");
                if (rxSocket != null)
                    await rxSocket.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        private static async Task<IRxSocketClient> ConnectAsync(Config config, CancellationToken ct, ILogger logger)
        {
            Exception? lastException = null;
            foreach (var port in config.Ports)
            {
                config.IPEndPoint.Port = port;
                try
                {
                    return await config.IPEndPoint.ConnectRxSocketClientAsync(logger, ct).ConfigureAwait(false);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.ConnectionRefused && e.SocketErrorCode != SocketError.TimedOut)
                        throw;
                    lastException = e;
                }
            }
            var ports = config.Ports.Select(p => p.ToString()).JoinStrings(", ");
            var message = $"Could not connect to TWS/Gateway at [{config.IPEndPoint.Address}]:{ports}.";
            throw new OperationCanceledException(message, lastException);
        }

        private static async Task Login(IRxSocketClient rxsocket, Config config, CancellationToken ct, ILogger logger)
        {
            // Send the first message without a length prefix.
            rxsocket.Send("API".ToBuffer());

            // Start sending and receiving messages with an int32 message length prefix (UseV100Plus).
            // Report a range of supported API versions to TWS.
            rxsocket.Send(new[]
                { $"v{Config.ServerVersionMin}..{Config.ServerVersionMax}" }
                    .ToBufferWithLengthPrefix());

            rxsocket.Send(new[]
                { ((int)RequestCode.StartApi).ToString(), "2", config.ClientId.ToString(), config.OptionalCapabilities }
                    .ToBufferWithLengthPrefix());

            var message1 = await rxsocket.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync().ConfigureAwait(false);
            // ServerVersion is the highest supported API version in the range specified.
            if (!Enum.TryParse(message1[0], out ServerVersion version))
                throw new InvalidDataException($"Could not parse server version '{message1[0]}'.");
            config.ServerVersionCurrent = version;
            config.Date = message1[1];
            logger.LogInformation($"Date: {config.Date}.");

            var message2 = await rxsocket.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync().ConfigureAwait(false);
            config.ManagedAccounts = message2[2];
            logger.LogInformation($"Managed Accounts: {config.ManagedAccounts}.");

            var message3 = await rxsocket.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync().ConfigureAwait(false);
            //config.NextIdValue = int.Parse(message3[2]) - 1;
            //var message4 = await rxsocket.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync().ConfigureAwait(false);
            ;
        }
    }
}
