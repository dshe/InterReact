using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIpEndPoint { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
    static IInterReactClient NullInstance { get; } = new NullInterReactClient();
}

file sealed class NullInterReactClient : IInterReactClient
{
    public IPEndPoint RemoteIpEndPoint => throw new InvalidOperationException();
    public Request Request => throw new InvalidOperationException();
    public IObservable<object> Response => throw new InvalidOperationException();
    public Service Service => throw new InvalidOperationException();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class InterReactClient : IInterReactClient
{
    private readonly Connection _connection;
    public IPEndPoint RemoteIpEndPoint => _connection.RemoteIpEndPoint;
    private readonly ServiceProvider _serviceProvider;
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }
    private InterReactClient(Connection connection, InterReactOptions options)
    {
        _connection = connection;
        _serviceProvider = new ServiceCollection()
            .AddSingleton(options)
            .AddSingleton(options.LogFactory) // add LoggerFactory before AddLogging()
            .AddLogging() // so that LogFactory is used rather than the LoggerFactory.
            .AddSingleton(connection) // instance
            .AddSingleton<Request>()
            .AddTransient<RequestMessage>() // transient!
            .AddSingleton<Func<RequestMessage>>(s => () => s.GetRequiredService<RequestMessage>()) // factory
            .AddSingleton<ResponseObservable>() // IObservable<object>
            .AddSingleton<ResponseComposer>()
            .AddSingleton<ResponseReader>()
            .AddSingleton<ResponseParser>()
            .AddSingleton<Service>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        Request = _serviceProvider.GetRequiredService<Request>();
        Response = _serviceProvider.GetRequiredService<ResponseObservable>();
        Service = _serviceProvider.GetRequiredService<Service>();
    }
    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
        await _connection.DisposeAsync().ConfigureAwait(false);
    }

    public static async Task<IInterReactClient> CreateAsync(Action<InterReactOptions>? action = null, CancellationToken ct = default)
    {
        InterReactOptions options = new();
        action?.Invoke(options);
        if (options.LogFactory == NullLoggerFactory.Instance && options.Logger != NullLogger.Instance)
            options.LogFactory = options.Logger.ToLoggerFactory();

        AssemblyName name = Assembly.GetExecutingAssembly().GetName();
        ILogger logger = options.LogFactory.CreateLogger<InterReactClient>();
        logger.LogInformation("{Name} v{Version}.", name.Name, name.Version);

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                    logger.LogCritical(ex, "Unhandled exception.");
                else
                    logger.LogCritical("Unhandled non-Exception object: {Object}.", e.ExceptionObject);
            }
            catch
            {
                // Last-chance handler must not throw.
            }
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            logger.LogError(e.Exception, "Unobserved task exception.");
            e.SetObserved();
        };

#pragma warning disable CA2000 // Dispose objects before losing scope
        Socket socket = await SocketConnector.ConnectAsync(options.TwsIpAddress, options.TwsPortAddresses, logger, ct).ConfigureAwait(false);
        SocketWrapper socketWrapper = new(socket);
        Connection connection = new(socketWrapper, options);
#pragma warning restore CA2000 // Dispose objects before losing scope
        try
        {
            await connection.StartAsync(ct).ConfigureAwait(false);
            return new InterReactClient(connection, options);
        }
        catch
        { 
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }
}
