using RxSockets;

namespace InterReact;

public sealed class Response : IObservable<object>
{
    private readonly IObservable<object> Observable;

    public Response(Connection connection, IRxSocketClient socketClient, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(socketClient);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        Observable = socketClient
            .ReceiveAllAsync
            .ToObservableFromAsyncEnumerable()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ComposeMessages(connection, loggerFactory)
            .LogMessages(loggerFactory)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => Observable.Subscribe(observer);
}
