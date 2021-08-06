using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RxSockets;

namespace InterReact
{
    public interface IInterReactClient: IAsyncDisposable, IEditorBrowsableNever
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
    }

    public class InterReactClient : IInterReactClient
    {
        private readonly IRxSocketClient RxSocket;
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }

        // must be public because it is constructed by the container
        public InterReactClient(IRxSocketClient rxsocket, Request request, 
            IObservable<object> response, Services services)
        {
            RxSocket = rxsocket;
            Request = request;
            Response = response;
            Services = services;
        }

        public async ValueTask DisposeAsync() => await RxSocket.DisposeAsync();
    }
}
