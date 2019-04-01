using System;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.Service;
using RxSockets;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace InterReact
{
    public interface IInterReactClient : IDisposable, IEditorBrowsableNever
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
        Config Config { get; }
        Task DisconnectAsync();
    }

    public sealed class InterReactClient : IInterReactClient
    {
        private readonly IRxSocketClient RxSocket;
        public Config Config { get; }
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }
        private readonly CancellationToken Ct;
        public Task DisconnectAsync() => RxSocket.DisconnectAsync(Ct);
        public void Dispose() => RxSocket.Dispose();

        internal InterReactClient(IRxSocketClient rxsocket, Config config, Request request, IObservable<object> response,
            Services services, ILoggerFactory loggerFactory, CancellationToken ct)
        {
            RxSocket = rxsocket;
            Config = config;
            Request = request;
            Response = response;
            Services = services;
            Ct = ct;
            var logger = loggerFactory.CreateLogger(GetType());
            logger.LogDebug($"Constructed {GetType().Name}.");
        }
    }
}
