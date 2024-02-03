using Microsoft.Extensions.Configuration;
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

public sealed class InterReactClient : IInterReactClient
{
    private readonly IRxSocketClient RxSocketClient;
    public IPEndPoint RemoteIpEndPoint => (IPEndPoint)RxSocketClient.RemoteEndPoint;
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }
    public InterReactClient(IRxSocketClient rxSocketClient, Request request, Response response, Service service)
    {
        RxSocketClient = rxSocketClient;
        Request = request;
        Response = response;
        Service = service;
    }

    public async ValueTask DisposeAsync() =>
        await RxSocketClient.DisposeAsync().ConfigureAwait(false);

    public static async Task<IInterReactClient> ConnectAsync(Action<InterReactOptions>? action = null, IConfigurationSection? configSection = null, CancellationToken ct = default)
    {
        InterReactOptions options = new(action, configSection);
        return await Connector.ConnectAsync(options, ct).ConfigureAwait(false);
    }
}

