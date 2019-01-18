using System;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.Service;
using RxSockets;
using System.Threading;

namespace InterReact
{
    public interface IInterReactClient : IDisposable, IEditorBrowsableNever
    {
        Request Request { get; }
        IObservable<object> Response { get; }
        Services Services { get; }
        Config Config { get; }
        Task DisconnectAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Example usage:
    /// var client = await InterReactClient.BuildAsync();
    /// var client = await InterReactClient.Builder.SetIpAddress(address).SetPort(port).BuildAsync();
    /// </summary>
    public sealed class InterReactClient : IInterReactClient
    {
        public static InterReactClientBuilder Builder => new InterReactClientBuilder();
        public static Task<IInterReactClient> BuildAsync(int timeout = 1000, CancellationToken ct = default) =>
            new InterReactClientBuilder().BuildAsync(timeout, ct);

        internal readonly IRxSocket rxSocket;
        public Config Config { get; }
        public Request Request { get; }
        public IObservable<object> Response { get; }
        public Services Services { get; }
        public Task DisconnectAsync(CancellationToken ct) => rxSocket.DisconnectAsync(ct);
        public void Dispose() => rxSocket.Dispose();

        internal InterReactClient(IRxSocket rxsocket, Config config, Request request, IObservable<object> response, Services services)
        {
            rxSocket = rxsocket;
            Config = config;
            Request = request;
            Response = response;
            Services = services;
        }
    }
}
