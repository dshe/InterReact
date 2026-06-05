using System.Net;
using Microsoft.Extensions.DependencyInjection;
namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIpEndPoint { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
}

public sealed class NullInterReactClient : IInterReactClient
{
    public static IInterReactClient Instance { get; } = new NullInterReactClient();
    public IPEndPoint RemoteIpEndPoint => throw new InvalidOperationException();
    public Request Request => throw new InvalidOperationException();
    public IObservable<object> Response => throw new InvalidOperationException();
    public Service Service => throw new InvalidOperationException();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class InterReactClient(
    Connection connection, Request request, Response response, Service service) : IInterReactClient
{
    private readonly Connection _connection = connection;
    public IPEndPoint RemoteIpEndPoint => _connection.RemoteEndPoint;
    public Request Request { get; } = request;
    public IObservable<object> Response { get; } = response;
    public Service Service { get; } = service;
    public async ValueTask DisposeAsync() =>
        await _connection.DisposeAsync().ConfigureAwait(false);
    public static async Task<IInterReactClient> CreateAsync(Action<InterReactOptions>? action = null, CancellationToken ct = default)
    {
        InterReactOptions options = new(action);
        Connection? connection = null;
        try
        {
#pragma warning disable CA2000 // Disposal is handled by InterReactClient.
            connection = await Connection.CreateAsync(options, ct).ConfigureAwait(false);
#pragma warning restore CA2000

            return new ServiceCollection()
                .AddSingleton(options)
                .AddSingleton(options.Clock)
                .AddSingleton(options.LogFactory) // add LoggerFactor before AddLogging()
                .AddLogging() // so that LogFactory is used rather than the LoggerFactory.
                .AddSingleton(connection) // instance
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
            if (connection != null)
                await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }
}


