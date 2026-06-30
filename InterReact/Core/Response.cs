namespace InterReact;

public sealed class Response : IObservable<object>
{
    private readonly IObservable<object> _observable;

    public Response(Connection connection, ResponseComposer responseComposer, ILogger<Response> logger)
    {
        ArgumentNullException.ThrowIfNull(connection, nameof(connection));

        _observable = connection
            .Observable
            .ComposeMessage(responseComposer, logger)
            .Log(logger)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => _observable.Subscribe(observer);
}
