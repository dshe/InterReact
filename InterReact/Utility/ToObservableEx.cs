using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
namespace InterReact;

// For requests that do not use requestId. 
public static partial class Ext
{
    // Usage: Returns a single result: CurrentTime.
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


    // Usage: Multiple results: OpenOrders.
    internal static IObservable<T> ToObservableMultiple<T, TEnd>(
        this IObservable<T> filteredSource, Action startRequest)
    {
        return Observable.Create<T>(observer =>
        {
            IDisposable subscription = filteredSource
                .SubscribeSafe(Observer.Create<T>(
                    onNext: o =>
                    {
                        if (o is TEnd)
                            observer.OnCompleted();
                        else
                            observer.OnNext(o); // IMPORTANT!
                        },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest();

            return subscription;
        });
    }


    // Usage: Continuous results: AccountUpdates, Positions.
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
}
