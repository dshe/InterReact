using RxSockets;
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
    IRxSocketClient rxSocketClient, Request request, Response response, Service service) : IInterReactClient
{
    private readonly IRxSocketClient RxSocketClient = rxSocketClient;
    public IPEndPoint RemoteIpEndPoint => (IPEndPoint)RxSocketClient.RemoteEndPoint;
    public Request Request { get; } = request;
    public IObservable<object> Response { get; } = response;
    public Service Service { get; } = service;
    public async ValueTask DisposeAsync() =>
        await RxSocketClient.DisposeAsync().ConfigureAwait(false);
    public static async Task<IInterReactClient> ConnectAsync(
        Action<InterReactOptions>? action = null, CancellationToken ct = default) =>
            await Connector.ConnectAsync(action, ct).ConfigureAwait(false);
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
