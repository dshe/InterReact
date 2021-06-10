using System;
using System.Threading.Tasks;
using RxSockets;

namespace InterReact
{
    public interface IInterReact: IAsyncDisposable, IEditorBrowsableNever
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
    }

    public sealed class InterReact : IInterReact
    {
        private readonly IRxSocketClient RxSocket;
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }

        // must be public because it is constructed by the container
        public InterReact(IRxSocketClient rxsocket, Request request, 
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
