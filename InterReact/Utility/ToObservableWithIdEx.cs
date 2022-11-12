using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact;

// Usage: for requests that use RequestId.
public static partial class Ext
{
    // Usage: Single result: FundamentalData, SymbolSamples.
    // may also return alerts(s)
    internal static IObservable<IHasRequestId> ToObservableSingleWithId(this IObservable<object> source,
        Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getNextId();
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        cancelable = false;
                        if (m is Alert alert)
                        {
                            if (alert.IsFatal)
                                observer.OnError(alert.ToException()); // IMPORTANT!
                            else
                                observer.OnNext(m);
                            return;
                        }
                        observer.OnNext(m);
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
                startRequest(id);
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (stopRequest is not null && cancelable is true)
                    stopRequest(id);
                subscription.Dispose();
            });
        });
    }


    // Usage: Multiple results: TickSnapshot, Executions.
    internal static IObservable<IHasRequestId> ToObservableMultipleWithId<TEnd>(
        this IObservable<object> source, Func<int> getNextId, Action<int> startRequest)
            where TEnd : IHasRequestId
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getNextId();

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is TEnd)
                            observer.OnCompleted();
                        else if (m is Alert alert && alert.IsFatal) // IMPORTANT!
                                observer.OnError(alert.ToException());
                        else
                            observer.OnNext(m);
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest(id);

            return subscription;
        });
    }


    // Usage: Continuous results: AccountSummary, Tick, MarketDepth.
    internal static IObservable<IHasRequestId> ToObservableContinuousWithId(this IObservable<object> source,
        Func<int> getNextId, Action<int> startRequest, Action<int> stopRequest)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getNextId();
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is Alert alert && alert.IsFatal)
                            observer.OnError(alert.ToException());
                        else
                            observer.OnNext(m);
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
                startRequest(id);
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (cancelable is true)
                    stopRequest(id);
                subscription.Dispose();
            });
        });
    }
}
