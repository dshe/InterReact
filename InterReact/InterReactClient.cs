namespace InterReact;

public interface IInterReactClient : IAsyncDisposable
{
    Connection Connection { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
}

public sealed class InterReactClient : IInterReactClient
{
    public Connection Connection { get; }
    public Request Request { get; }
    public IObservable<object> Response { get; }
    public Service Service { get; }

    public InterReactClient(Connection connection, Request request, Response response, Service service)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        Request = request;
        Response = response;
        Service = service;
    }

    public async ValueTask DisposeAsync() =>
        await Connection.DisposeAsync().ConfigureAwait(false);
}
