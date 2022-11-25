using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
    private readonly InterReactClientConnector ClientConnector;
    public IPEndPoint RemoteIPEndPoint { get; }
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }

    // InterReactClientConnector.ConnectAsync() calls this constructor.
    // The class must be public since it is resolved through Microsoft dependency injection.
    public InterReactClient(InterReactClientConnector connector, Request request, IObservable<object> response, Service service)
    {
        ArgumentNullException.ThrowIfNull(connector, nameof(connector));
        ArgumentNullException.ThrowIfNull(connector.RxSocketClient, nameof(connector.RxSocketClient));
        ClientConnector = connector;
        RemoteIPEndPoint = connector.RxSocketClient.RemoteIPEndPoint;
        Request = request;
        Response = response;
        Service = service;
    }

    public async ValueTask DisposeAsync() =>
        await ClientConnector.RxSocketClient!.DisposeAsync().ConfigureAwait(false);
}

public static partial class Ext
{
    public static bool IsIBDemoPort(this int port) =>
        port == (int)IBDefaultPort.TwsDemoAccount || port == (int)IBDefaultPort.GatewayDemoAccount;
    public static IBDefaultPort GetIBDefaultPort(this int port) =>
        Enum.GetValues<IBDefaultPort>().Where(v => (int)v == port).SingleOrDefault(IBDefaultPort.None);
}
