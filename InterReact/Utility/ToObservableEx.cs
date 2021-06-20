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
        // Returns a single result: CurrentTime, FinancialAdvisor, ManagedAccounts, ScannerParameters.
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


        // Multiple results: OpenOrders.
        internal static IObservable<object> ToObservableMultiple<TEnd>(
            this IObservable<object> filteredSource, Action startRequest)
        {
            return Observable.Create<object>(observer =>
            {
                IDisposable subscription = filteredSource
                    .SubscribeSafe(Observer.Create<object>(
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


        // Continuous results: AccountUpdates, NewsBulletins, Positions.
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
