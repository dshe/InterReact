using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact;

// Usage: for requests that use RequestId.
public static partial class Extensionz
{
    // Usage: Single result: SymbolSamples.
    // may also return alerts(s)
    internal static IObservable<IHasRequestId> ToObservableSingleWithRequestId(this IObservable<object> source,
        Func<int> getRequestId, Action<int> startRequest, Action<int>? stopRequest = null)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int requestId = getRequestId();
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == requestId)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        cancelable = false;
                        observer.OnNext(m);
                        if (m is Alert alert && !alert.IsFatal)
                            return;
                        observer.OnCompleted();
                    },
                    onError: e =>
                    {
                        cancelable = false;
                        observer.OnError(e);
                    },
                    onCompleted: () =>
                    {
                        cancelable = false;
                        observer.OnCompleted();
                    }));

            if (cancelable is null)
                startRequest(requestId);
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (stopRequest is not null && cancelable is true)
                    stopRequest(requestId);
                subscription.Dispose();
            });
        });
    }


    // Usage: Multiple results: TickSnapshot
    internal static IObservable<IHasRequestId> ToObservableMultipleWithRequestId<TEnd>(
        this IObservable<object> source, Func<int> getRequestId, Action<int> startRequest)
            where TEnd : IHasRequestId
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int requestId = getRequestId();

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == requestId)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is TEnd)
                        {
                            observer.OnCompleted();
                            return;
                        }
                        observer.OnNext(m);
                        if (m is Alert alert && alert.IsFatal) // IMPORTANT!
                            observer.OnCompleted();
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest(requestId);

            return subscription;
        });
    }


    // Usage: Continuous results: AccountSummary, Tick.
    internal static IObservable<IHasRequestId> ToObservableContinuousWithRequestId(this IObservable<object> source,
        Func<int> getRequestId, Action<int> startRequest, Action<int> stopRequest)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int requestId = getRequestId();
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == requestId)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        observer.OnNext(m);
                        if (m is Alert alert && alert.IsFatal)
                            observer.OnCompleted();
                    },
                    onError: e =>
                    {
                        cancelable = false;
                        observer.OnError(e);
                    },
                    onCompleted: () =>
                    {
                        cancelable = false;
                        observer.OnCompleted();
                    }));

            if (cancelable is null)
                startRequest(requestId);
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (cancelable is true)
                    stopRequest(requestId);
                subscription.Dispose();
            });
        });
    }
}
