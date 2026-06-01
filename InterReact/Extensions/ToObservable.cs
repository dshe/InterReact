using System.Reactive;
using System.Reactive.Disposables;
namespace InterReact;

public static partial class Extension
{
    public static IObservable<T> ToObservable<T>(
        this IObservable<T> source,
        Action startRequest)
    {
        return Observable.Create<T>(observer =>
        {
            IDisposable subscription = source
                .SubscribeSafe(Observer.Create<T>(
                    onNext: m =>
                    {
                        observer.OnNext(m);
                        observer.OnCompleted();
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest();

            return subscription;
        });
    }

    public static IObservable<T> ToObservable<T>(
        this IObservable<object> source,
        Func<ValueTask> startRequest, Func<ValueTask>? stopRequest = null)
    {
        return Observable.Create<T>(async observer =>
        {
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<T>()
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
                await startRequest().ConfigureAwait(false);
            cancelable ??= true;

            return Disposable.Create(async () =>
            {
                if (cancelable is true && stopRequest is not null)
                    await stopRequest().ConfigureAwait(false);
                subscription.Dispose();
            });
        });
    }

    //Func<ValueTask>
    // For continuous results with RequestId: AccountUpdatesMulti, MarketData
    // For multiple   results with RequestId: ContractDetails, MarketDataSnapshot
    public static IObservable<IHasRequestId> ToObservableWithId(
        this IObservable<object> source, 
        Func<int> getRequestId,
        Func<int,ValueTask> startRequest, Func<int, ValueTask>? stopRequest = null)
    {
        return Observable.Create<IHasRequestId>(async observer =>
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
                await startRequest(id).ConfigureAwait(false);
            cancelable ??= true;

            return Disposable.Create(async () =>
            {
                if (cancelable is true && stopRequest is not null)
                    await stopRequest(id).ConfigureAwait(false);
                subscription.Dispose();
            });
        });
    }

}
