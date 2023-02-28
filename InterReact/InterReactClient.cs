using System.Net;

namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIpEndPoint { get; }
    public ServerVersion ServerVersion { get; }
    int ClientId { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
}

public sealed class InterReactClient : IInterReactClient
{
    private readonly Connection Connection;
    public IPEndPoint RemoteIpEndPoint { get; }
    public ServerVersion ServerVersion { get; }
    public int ClientId { get; }
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }

    public InterReactClient(Connection connection, Request request, Response response, Service service)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        RemoteIpEndPoint = connection.RemoteIpEndPoint;
        ServerVersion = connection.ServerVersionCurrent;
        ClientId = connection.ClientId;
        Request = request;
        Response = response;
        Service = service;
    }

    public async ValueTask DisposeAsync() =>
        await Connection.DisposeAsync().ConfigureAwait(false);
}
