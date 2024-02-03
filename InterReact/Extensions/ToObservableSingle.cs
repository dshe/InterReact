using System.Reactive;
using System.Reactive.Disposables;

namespace InterReact;

public static partial class Extension
{
    // Usage: returns a single result:.
    internal static IObservable<T> ToObservableSingle<T>
        (this IObservable<object> unfilteredSource, Action startRequest)
    {
        return Observable.Create<T>(observer =>
        {
            IDisposable subscription = unfilteredSource
                .OfType<T>() // IMPORTANT!
                .SubscribeSafe(Observer.Create<T>(
                    onNext: t =>
                    {
                        observer.OnNext(t);
                        observer.OnCompleted();
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest();

            return subscription;
        });
    }

    // Usage: single result
    internal static IObservable<IHasRequestId> ToObservableSingleWithId(this IObservable<object> source,
        Func<int> getId, Action<int> startRequest, Action<int>? stopRequest = null)
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getId();
            bool? cancelable = null;

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        cancelable = false;
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
}
