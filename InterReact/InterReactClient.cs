using System.Net;
using System.Threading.Tasks;
using RxSockets;
namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIPEndPoint { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Svc Services { get; }
}

public sealed class InterReactClient : IInterReactClient
{
    private readonly IRxSocketClient RxSocket;
    public IPEndPoint RemoteIPEndPoint => RxSocket.RemoteIPEndPoint;
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Svc Services { get; }

    // This constructor must be public since it is constructed by the container.
    public InterReactClient(IRxSocketClient rxsocket, Request request, IObservable<object> response, Svc services)
    {
        ArgumentNullException.ThrowIfNull(rxsocket);
        RxSocket = rxsocket;
        Request = request;
        Response = response;
        Services = services;
    }

    public async ValueTask DisposeAsync() => await RxSocket.DisposeAsync().ConfigureAwait(false);
}

public static class Xtensions
{
    public static bool IsDemoPort(this int port) => port == (int)DefaultPort.TwsDemoAccount || port == (int)DefaultPort.GatewayDemoAccount;
}
