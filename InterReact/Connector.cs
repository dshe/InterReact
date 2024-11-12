using Microsoft.Extensions.DependencyInjection;
using RxSockets;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net;
using Stringification;
using System.Reflection;
using System.Reactive.Threading.Tasks;
namespace InterReact;

internal sealed class Connector
{
    private InterReactOptions Options { get; }
    private ILogger Logger { get; }

    private Connector(InterReactOptions options)
    {
        Options = options;
        Logger = options.LogFactory.CreateLogger<Connector>();
        AssemblyName name = Assembly.GetExecutingAssembly().GetName();
        Logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);
    }
    internal static async Task<IInterReactClient> ConnectAsync(Action<InterReactOptions>? action, CancellationToken ct)
    {
        InterReactOptions options = new(action);
        Connector connector = new(options);
        IInterReactClient client = await connector.ConnectAsync(ct).ConfigureAwait(false);
        return client;
    }

    private async Task<IInterReactClient> ConnectAsync(CancellationToken ct)
    {
        IRxSocketClient rxSocketClient = NullRxSocketClient.Instance;
        try
        {
            rxSocketClient = await ConnectSocketAsync(ct).ConfigureAwait(false);
            await Login(rxSocketClient, ct).ConfigureAwait(false);
            Logger.LogInformation("Logged into TWS/Gateway using clientId: {ClientId} and server version: {ServerVersionCurrent}.",
                Options.TwsClientId, (int)Options.ServerVersionCurrent);

            return new ServiceCollection()
                .AddSingleton(Options)
                .AddSingleton(Options.Clock)
                .AddSingleton(Options.LogFactory) // add LoggerFactor before AddLogging()
                .AddLogging() // so that LogFactory is used rather than the LoggerFactory.
                .AddSingleton(rxSocketClient) // instance
                .AddSingleton(new RingLimiter(Options.MaxRequestsPerSecond))
                .AddSingleton<Stringifier>()
                .AddSingleton<Request>()
                .AddTransient<RequestMessage>() // transient!
                .AddSingleton<Func<RequestMessage>>(s => () => s.GetRequiredService<RequestMessage>()) // factory
                .AddSingleton<Response>() // IObservable<object>
                .AddSingleton<ResponseReader>()
                .AddSingleton<ResponseParser>()
                .AddSingleton<ResponseMessageComposer>()
                .AddSingleton<Service>()
                .AddSingleton<IInterReactClient, InterReactClient>()
                .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true })
                .GetRequiredService<IInterReactClient>();
        }
        catch (Exception)
        {
            await rxSocketClient.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private async Task<IRxSocketClient> ConnectSocketAsync(CancellationToken ct)
    {
        IPEndPoint ipEndPoint = new(Options.TwsIpAddress, 0);
        foreach (int port in Options.IBPortAddresses)
        {
            ipEndPoint.Port = port;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(1));
            try
            {
                return await ipEndPoint.CreateRxSocketClientAsync(Options.LogFactory, cts.Token).ConfigureAwait(false);
                // token cancel  => OperationCanceledException
                // token timeout => OperationCanceledException
                // socket timeout/error => SocketException
            }
            catch (OperationCanceledException)
            {
                if (ct.IsCancellationRequested)
                    throw;
            }
            //catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused || e.SocketErrorCode == SocketError.TimedOut)
            catch (SocketException) { }
            finally
            {
                cts.Dispose();
            }
        }
        string message = $"Could not connect to TWS/Gateway at " +
            $"[{ipEndPoint.Address}]:{string.Join(", ", Options.IBPortAddresses)}. " +
            $"In TWS, navigate to Edit / Global Configuration / API / Settings and ensure" +
            $" the option \"Enable ActiveX and Socket Clients\" is selected.";
        throw new ArgumentException(message);
    }

    private async Task Login(IRxSocketClient rxSocketClient, CancellationToken ct)
    {
        // send without prefix
        rxSocketClient.Send("API".ToByteArray());

        // Report a range of supported API versions to TWS.
        SendWithPrefix($"v{(int)Options.ServerVersionMin}..{(int)Options.ServerVersionMax}");

        SendWithPrefix(
            ((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
            "2",
            Options.TwsClientId.ToString(CultureInfo.InvariantCulture), 
            Options.OptionalCapabilities);

        string[] message;
        try
        {
            message = await rxSocketClient
                .ReceiveObservable
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync()
                .ToTask(ct)
                .ConfigureAwait(false);
        }
        catch (TimeoutException e)
        {
            throw new TimeoutException("Timeout waiting for the first response from TWS/Gateway. Try restarting.", e);
        }

        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(message[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{message[0]}'.");
        Options.ServerVersionCurrent = version;
        if (Options.ServerVersionCurrent < Options.ServerVersionMin || Options.ServerVersionCurrent > Options.ServerVersionMax)
            throw new InvalidDataException($"Invalid server version '{Options.ServerVersionCurrent}'.");

        // message[1] is Date

        // local method
        void SendWithPrefix(params string[] strings) => rxSocketClient
            .Send(strings.ToByteArray().ToByteArrayWithLengthPrefix());
    }
}
