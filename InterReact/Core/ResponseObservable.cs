namespace InterReact;

public sealed class ResponseObservable : IObservable<object>
{
    private readonly IObservable<object> _observable;

    public ResponseObservable(Connection connection, ResponseComposer responseComposer, ILogger<ResponseObservable> logger)
    {
        ArgumentNullException.ThrowIfNull(connection, nameof(connection));

        _observable = connection
            .ConnectionObservable
            .ComposeMessage(responseComposer)
            .Log(logger, connection.Ct)
            .Publish()
            .AutoConnect(); // Connect on first observer. Never disconnect.
    }

    public IDisposable Subscribe(IObserver<object> observer) => _observable.Subscribe(observer);
}
