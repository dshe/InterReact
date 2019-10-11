using System;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.Service;
using RxSockets;
using System.Threading;
using Microsoft.Extensions.Logging;

#nullable enable

namespace InterReact
{
    public interface IInterReactClient : IEditorBrowsableNever, IDisposable
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
        Config Config { get; }
    }

    public sealed class InterReactClient : IInterReactClient
    {
        private readonly IRxSocketClient RxSocket;
        public Config Config { get; }
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }
        public void Dispose() => RxSocket.Dispose();

        // constructed by container, so must be public
        public InterReactClient(IRxSocketClient rxsocket, Config config, Request request, IObservable<object> response,
            Services services, ILoggerFactory loggerFactory)
        {
            RxSocket = rxsocket;
            Config = config;
            Request = request;
            Response = response;
            Services = services;
            var logger = loggerFactory.CreateLogger(GetType());
            logger.LogDebug($"Constructed {GetType().Name}.");
        }
    }
}
