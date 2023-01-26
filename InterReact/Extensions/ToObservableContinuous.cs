using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact;

public static partial class Extension
{
    // For continuous results that do not use RequestId: AccountUpdates, AccountPositions.
    internal static IObservable<T> ToObservableContinuous<T>(this IObservable<T> filteredSource,
        Action startRequest, Action stopRequest)
    {
        return Observable.Create<T>(observer =>
        {
            bool? cancelable = null;

            IDisposable subscription = filteredSource
                .SubscribeSafe(Observer.Create<T>(
                    onNext: m => observer.OnNext(m),
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
    internal static IObservable<object> ToObservableContinuousWithId(this IObservable<object> source,
        Func<int> getRequestId, Action<int> startRequest, Action<int> stopRequest)
    {
        return Observable.Create<object>(observer =>
        {
            int id = getRequestId();
            bool? cancelable = null;

            IDisposable subscription = source
                .WithRequestId(id)
                .SubscribeSafe(Observer.Create<object>(
                    onNext: m =>
                    {
                        observer.OnNext(m); // messages and possibly alert(s)
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
