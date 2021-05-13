using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RxSockets;

namespace InterReact
{
    public interface IInterReactClient : IEditorBrowsableNever
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
        Config Config { get; }
        Task DisposeAsync();
    }

    public sealed class InterReactClient : IInterReactClient
    {
        private readonly IRxSocketClient RxSocket;
        public Config Config { get; }
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }
        public Task DisposeAsync() => RxSocket.DisposeAsync();

        // must be public because it is constructed by the container
        public InterReactClient(IRxSocketClient rxsocket, Config config, Request request, IObservable<object> response,
            Services services, ILogger logger)
        {
            RxSocket = rxsocket;
            Config = config;
            Request = request;
            Response = response;
            Services = services;
            logger.LogInformation($"Constructed {GetType().Name}.");
        }
    }
}
