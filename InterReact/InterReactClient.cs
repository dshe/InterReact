using System;
using System.Threading.Tasks;
using RxSockets;

namespace InterReact
{
    public interface IInterReactClient: IAsyncDisposable, IEditorBrowsableNever
    {
        Config Config { get; }
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
    }

    public sealed class InterReactClient : IInterReactClient
    {
        private readonly IRxSocketClient RxSocket;
        public Config Config { get; }
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }

        // must be public because it is constructed by the container
        public InterReactClient(IRxSocketClient rxsocket, Config config, Request request, 
            IObservable<object> response, Services services)
        {
            RxSocket = rxsocket;
            Config = config;
            Request = request;
            Response = response;
            Services = services;
        }

        public async ValueTask DisposeAsync() => await RxSocket.DisposeAsync();
    }
}
