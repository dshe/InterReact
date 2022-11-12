using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RxSockets;

namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIPEndPoint { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
}

public sealed class InterReactClient : IInterReactClient
{
    private readonly IRxSocketClient RxSocket;
    public IPEndPoint RemoteIPEndPoint => RxSocket.RemoteIPEndPoint;
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }

    // This constructor is called by InterReactClientConnector.ConnectAsync().
    // Must be public because it is called through Microsoft dependency injection.
    public InterReactClient(IRxSocketClient rxsocket, Request request, IObservable<object> response, Service service)
    {
        RxSocket = rxsocket;
        Request = request;
        Response = response;
        Service = service;
    }

    public async ValueTask DisposeAsync() => await RxSocket.DisposeAsync().ConfigureAwait(false);
}

public static partial class Ext
{
    public static IBDefaultPort GetIBDefaultPort(this int port) =>
        Enum.GetValues<IBDefaultPort>().Where(v => (int)v == port).SingleOrDefault(IBDefaultPort.None);

    public static bool IsIBDemoPort(this int port)
    {
        IBDefaultPort portType = GetIBDefaultPort(port);
        return portType == IBDefaultPort.TwsDemoAccount || portType == IBDefaultPort.GatewayDemoAccount;
    }
}
