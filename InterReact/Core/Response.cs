using RxSockets;
using Stringification;
using System.Reactive.Concurrency;

namespace InterReact;

public sealed class Response : IObservable<object>
{
    private IObservable<object> Observable { get; }

    public Response(ILogger<Response> logger, IRxSocketClient socketClient, ResponseMessageComposer composer, Stringifier stringifier)
    {
        ArgumentNullException.ThrowIfNull(socketClient);

        Observable = socketClient
            .ReceiveAllAsync
            .ToObservableFromAsyncEnumerable(NewThreadScheduler.Default)
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ComposeMessage(composer)
            .LogMessages(logger, stringifier)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => Observable.Subscribe(observer);
}

public static partial class Extension
{
    internal static IObservable<object> ComposeMessage(this IObservable<string[]> source, ResponseMessageComposer composer)
    {
        return Observable.Create<object>(observer =>
        {
            return source.Subscribe(
                message =>
                {
                    try
                    {
                        composer.OnNext(message, observer.OnNext);
                    }
#pragma warning disable CA1031
                    catch (Exception e)
#pragma warning restore CA1031
                    {
                        observer.OnError(e);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
        });
    }

    internal static IObservable<object> LogMessages(this IObservable<object> source, ILogger logger, Stringifier stringifier)
    {
        return source.Do(msg =>
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.Log(LogLevel.Debug, "Response {Message}", stringifier.Stringify(msg));
        });
    }
}
