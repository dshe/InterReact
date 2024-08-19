using RxSockets;
using Stringification;
using System.Reactive;

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
            .ComposeMessage(composer)
            .LogMessage(logger, stringifier)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => Observable.Subscribe(observer);
}

public static partial class Extension
{
    internal static IObservable<object> ComposeMessage(this IObservable<string[]> source, ResponseMessageComposer composer) =>
        Observable.Create<object>(observer =>
            source.SubscribeSafe(Observer.Create<string[]>(msg => composer.OnNext(msg, observer.OnNext))));

    internal static IObservable<object> LogMessage(this IObservable<object> source, ILogger logger, Stringifier stringifier) =>
        source.Do(msg =>
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.Log(LogLevel.Debug, "Response received: {Message}", stringifier.Stringify(msg));
        });
}
