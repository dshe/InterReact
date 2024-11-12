using RxSockets;
using Stringification;
using System.Diagnostics;
using System.Reactive.Concurrency;
namespace InterReact;

public sealed class Response : IObservable<object>
{
    private IObservable<object> Observable { get; }

    public Response(ILogger<Response> logger, IRxSocketClient socketClient, ResponseMessageComposer composer, Stringifier stringifier)
    {
        ArgumentNullException.ThrowIfNull(socketClient);

        Observable = socketClient
            .ReceiveObservable
            .SubscribeOn(NewThreadScheduler.Default.BackgroundThread("SubscriberThread"))
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ComposeMessage(composer)
            .Do(msg => logger.LogResponseMessage(stringifier.Stringify(msg)))
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
            return source.Subscribe(onNext: strings =>
            {
                try
                {
                    object result = composer.ComposeMessage(strings);
                    if (result is object[] results)
                    {
                        Debug.Assert(strings[0] == "1");
                        Debug.Assert(results.Length == 2);
                        observer.OnNext(results[0]); // priceTick
                        observer.OnNext(results[1]); // sizeTick
                        return;
                    }
                    observer.OnNext(result);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            },
            onError: observer.OnError,
            onCompleted: observer.OnCompleted);
        });
    }
}
