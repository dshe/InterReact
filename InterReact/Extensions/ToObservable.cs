using System.Reactive;
using System.Reactive.Disposables;

namespace InterReact;

public static partial class Extension
{
    // For continuous results: AccountUpdates, AccountPositions.
    internal static IObservable<T> ToObservableContinuous<T>(
        this IObservable<T> filteredSource, Action startRequest, Action stopRequest)
    {
        return Observable.Create<T>(observer =>
        {
            bool? cancelable = null;

            IDisposable subscription = filteredSource
                .SubscribeSafe(Observer.Create<T>(
                    onNext: observer.OnNext,
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
                startRequest();
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (cancelable is true)
                    stopRequest();
                subscription.Dispose();
            });
        });
    }

    // For continuous results with RequestId: AccountSummary, Tick.
    internal static IObservable<IHasRequestId> ToObservableContinuousWithId(
        this IObservable<object> source, Func<int> getRequestId, Action<int> startRequest, Action<int> stopRequest)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getRequestId();
            bool? cancelable = null;

            IDisposable subscription = source
                .WithRequestId(id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: observer.OnNext,
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

    // For multiple results with RequestId: MarketDataSnapshot
    internal static IObservable<IHasRequestId> ToObservableMultipleWithId<TEnd>(
        this IObservable<object> source, Func<int> getRequestId, Action<int> startRequest)
            where TEnd : IHasRequestId
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getRequestId();

            IDisposable subscription = source
                .WithRequestId(id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is not TEnd)
                            observer.OnNext(m);
                        else
                            observer.OnCompleted();
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest(id);

            return subscription;
        });
    }
}
