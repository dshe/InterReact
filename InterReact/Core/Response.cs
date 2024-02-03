using RxSockets;
using Stringification;

namespace InterReact;

public sealed class Response : IObservable<object>
{
    private IObservable<object> Observable { get; }

    public Response(ILogger<Response> logger, IRxSocketClient socketClient, ResponseMessageComposer composer, Stringifier stringifier)
    {
        ArgumentNullException.ThrowIfNull(socketClient);

        Observable = socketClient
            .ReceiveAllAsync
            .ToObservableFromAsyncEnumerable()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ComposeMessages(composer)
            .LogMessages(logger, stringifier)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => Observable.Subscribe(observer);
}
