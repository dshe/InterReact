using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RxSockets;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Extensions;
using InterReact.Service;
using NodaTime;
using InterReact.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InterReact
{
    public sealed class InterReactClientBuilder : EditorBrowsableNever
    {
        private readonly ILogger Logger;
        private readonly CancellationToken Ct;
        private readonly ServiceCollection InjectedServices = new ServiceCollection();
        private readonly Config Config = new Config();

        // ctor, with optional external services
        public InterReactClientBuilder(ILoggerFactory? loggerFactory = null, CancellationToken ct = default)
        {
            Ct = ct;
            loggerFactory ??= NullLoggerFactory.Instance;
            InjectedServices.AddSingleton(loggerFactory);
            Logger = loggerFactory.CreateLogger(GetType());
            Logger.LogDebug($"Constructing {GetType()}.");
            var name = GetType().GetTypeInfo().Assembly.GetName();
            Logger.LogInformation($"{name.Name} version {name.Version}.");
        }

        /// <summary>
        /// Specify an IPAddress to connect to TWS/Gateway.
        /// </summary>
        public InterReactClientBuilder SetIpAddress(IPAddress address)
        {
            Config.IPEndPoint.Address = address ?? throw new ArgumentNullException(nameof(address));
            return this;
        }

        /// <summary>
        /// Specify one or more ports to attempt connection to TWS/Gateway.
        /// Otherwise, connection will be attempted on ports 4001, 4002, 7496 and 7497.
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
            Config.Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            return this;
        }

        /////////////////////////////////////////////////////////////

        public async Task<IInterReactClient> BuildAsync(int timeout = 1000)
        {
            IRxSocketClient? rxSocket = null;

            try
            {
                rxSocket = await ConnectAsync(timeout).ConfigureAwait(false);
                Logger.LogInformation($"Connected to server at {Config.IPEndPoint}.");

                await Login(rxSocket, timeout).ConfigureAwait(false);
                Logger.LogInformation($"Logged into Tws/Gateway with ClientId: {Config.ClientId}, ServerVersion: {Config.ServerVersionCurrent}.");

                var response = rxSocket
                    .ReceiveObservable
                    .ToByteArrayOfLengthPrefix()
                    .ToStringArray()
                    .ToMessages(Config)
                    .Publish()
                    .RefCount();

                return InjectedServices
                    .AddSingleton(typeof(CancellationToken), Ct)
                    .AddSingleton(Config) // Config has no dependencies
                    .AddSingleton(rxSocket) // rxSocket is an instance of RxSocketClient
                    .AddSingleton<IObservable<object>>(response) // response is IPbservable<object>
                    .AddSingleton(new Limiter(Config.MaxRequestsPerSecond)) // configured instance
                    .AddTransient<RequestMessage>() // transient
                    .AddSingleton<Func<RequestMessage>>(x => () => x.GetService<RequestMessage>()) // factory
                    .AddSingleton<Request>()
                    .AddSingleton<Services>()
                    .AddSingleton<IInterReactClient, InterReactClient>()
                    .BuildServiceProvider()
                    .GetService<IInterReactClient>();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "InterReactClientBuilder.BuildAsync");
                rxSocket?.Dispose();
                throw;
            }
        }

        private async Task<IRxSocketClient> ConnectAsync(int timeout)
        {
            (SocketError error, IRxSocketClient? socket) result = default;
            foreach (var port in Config.Ports)
            {
                Config.IPEndPoint.Port = port;
                result = await TryConnectAsync(timeout).ConfigureAwait(false);
                if (result.error == SocketError.Success)
                    return result.socket ?? throw new ArgumentNullException();
            }
            // SocketException is not thrown here because it does not have a message property.
            var ports = Config.Ports.Select(p => p.ToString()).JoinStrings(", ");
            var message = $"{result.error.ToString()}.{Environment.NewLine}Could not connect to TWS/Gateway at [{Config.IPEndPoint.Address}]:{ports}.";
            Logger.LogCritical(message);
            if (result.error == SocketError.TimedOut)
                throw new TimeoutException(message);
            throw new IOException(message, new SocketException((int)result.error));
        }

        private async Task<(SocketError error, IRxSocketClient? rxsocket)> TryConnectAsync(int timeout)
        {
            try
            {
                var rxsocket = await RxSocketClient.ConnectAsync(Config.IPEndPoint, timeout).ConfigureAwait(false);
                return (SocketError.Success, rxsocket);
            }
            catch (SocketException se) when (se.ErrorCode == (int)SocketError.ConnectionRefused)
            {
                return (SocketError.ConnectionRefused, null);
            }
            catch (SocketException se) when (se.ErrorCode == (int)SocketError.TimedOut)
            {
                return (SocketError.TimedOut, null);
            }
        }

        private async Task Login(IRxSocketClient rxsocket, int timeout)
        {
            var ts = timeout >= 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.MaxValue;

            // Send the first message without a length prefix.
            "API".ToByteArray().SendTo(rxsocket);

            // Start sending and receiving messages with an int32 message length prefix (UseV100Plus).
            // Report a range of supported API versions to TWS.
            new[] { $"v{Config.ServerVersionMin}..{Config.ServerVersionMax}" }
            .ToByteArrayWithLengthPrefix()
            .SendTo(rxsocket);

            new[] { ((int)RequestCode.StartApi).ToString(), "2", Config.ClientId.ToString(), Config.OptionalCapabilities }
            .ToByteArrayWithLengthPrefix()
            .SendTo(rxsocket);

            var messages = await rxsocket
                .ReceiveObservable
                .ToByteArrayOfLengthPrefix()
                .ToStringArray()
                .Take(3)
                .Timeout(ts, Observable.Throw<string[]>(new TimeoutException("No response received from server.")))
                .ToList()
                .ToTask(Ct);

            // ServerVersion is the highest supported API version in the range specified.
            if (!Enum.TryParse(messages[0][0], out ServerVersion version))
                throw new InvalidDataException($"Could not parse server version '{messages[0][0]}'.");
            Config.ServerVersionCurrent = version;
            Config.Date = messages[0][1];
            Logger.LogInformation($"Date: {Config.Date}.");
            Config.ManagedAccounts = messages[1][2];
            Logger.LogInformation($"Managed Accounts: {Config.ManagedAccounts}.");
            Config.NextIdValue = int.Parse(messages[2][2]) - 1;
        }
    }
}
