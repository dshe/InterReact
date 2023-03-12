using RxSockets;

namespace InterReact;

public sealed class Response : IObservable<object>
{
    private readonly IObservable<object> Observable;

    public Response(IClock clock, ILoggerFactory loggerFactory, IRxSocketClient socketClient, Connection connection)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(socketClient);

        Observable = socketClient
            .ReceiveAllAsync
            .ToObservableFromAsyncEnumerable()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ComposeMessages(clock, loggerFactory, connection)
            .LogMessages(loggerFactory)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => Observable.Subscribe(observer);
}
