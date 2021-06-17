using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact
{
    // For requests that do not use requestId. 
    public static partial class Extensions
    {
        // Returns a single result: CurrentTime, ManagedAccounts, ScannerParameters
        internal static IObservable<T> ToObservableSingle<T>
            (this IObservable<object> source, Action startRequest)
        {
            return Observable.Create<T>(observer =>
            {
                IDisposable subscription = source
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


        // Multiple results: AccountPositions, OpenOrders
        internal static IObservable<object> ToObservableMultiple<TEnd>(this IObservable<object> filteredSource,
            Action startRequest, Action? stopRequest = null)
        {
            return Observable.Create<object>(observer =>
            {
                bool? cancelable = null;

                IDisposable subscription = filteredSource
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<object>(
                        onNext: o =>
                        {
                            if (o is TEnd)
                            {
                                cancelable = false;
                                observer.OnCompleted();
                                return;
                            }
                            observer.OnNext(o);
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                if (cancelable == null)
                    startRequest();
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest?.Invoke();
                    subscription.Dispose();
                });
            });
        }


        // Continuous results: AccountUpdate, NewsBulletins
        internal static IObservable<T> ToObservableContinuous<T>(this IObservable<T> filteredSource,
            Action startRequest, Action stopRequest)
        {
            return Observable.Create<T>(observer =>
            {
                bool? cancelable = null;

                IDisposable subscription = filteredSource
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(observer);

                if (cancelable == null)
                    startRequest();
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest.Invoke();
                    subscription.Dispose();
                });
            });
        }
    }
}
