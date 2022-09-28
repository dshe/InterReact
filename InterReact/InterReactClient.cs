﻿using System.Threading.Tasks;
using RxSockets;
namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    Request Request { get; }
    IObservable<object> Response { get; }
    Svc Services { get; }
}

public sealed class InterReactClient : IInterReactClient
{
    private readonly Func<ValueTask> Dispose;
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Svc Services { get; }

    // This constructor must be public since it is constructed by the container.
    public InterReactClient(IRxSocketClient rxsocket, Request request,
        IObservable<object> response, Svc services)
    {
        ArgumentNullException.ThrowIfNull(rxsocket);

        Dispose = rxsocket.DisposeAsync;
        Request = request;
        Response = response;
        Services = services;
    }

    public async ValueTask DisposeAsync() => await Dispose().ConfigureAwait(false);
}
