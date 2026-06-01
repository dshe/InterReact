using System.Net;
namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIpEndPoint { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
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
    public static async Task<IInterReactClient> CreateAsync(
        Action<InterReactOptions>? action = null, CancellationToken ct = default) =>
            await Connector.CreateClientAsync(new InterReactOptions(action), ct).ConfigureAwait(false);
}

public sealed class NullInterReactClient : IInterReactClient
{
    public static IInterReactClient Instance { get; } = new NullInterReactClient();
    public IPEndPoint RemoteIpEndPoint => throw new InvalidOperationException();
    public Request Request => throw new InvalidOperationException();
    public IObservable<object> Response => throw new InvalidOperationException();
    public Service Service => throw new InvalidOperationException();
    public ValueTask DisposeAsync() =>ValueTask.CompletedTask;
}
