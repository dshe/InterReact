using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact
{
    // For requests that use RequestId.
    public static partial class Extensions
    {
        // Single result: HistoricalData, FundamentalData, ScannerData
        internal static IObservable<T> ToObservableWithIdSingle<T>(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
                where T : IHasRequestId
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<T>()
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<T>(
                        onNext: m =>
                        {
                            observer.OnNext(m);
                            if (m is Alert)
                                return;
                            cancelable = false;
                            observer.OnCompleted();
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                if (cancelable == null)
                    startRequest(requestId);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest?.Invoke(requestId);
                    subscription.Dispose();
                });
            });
        }


        // Multiple results: TickSnapshot, ContractData, AccountSummary, Executions
        internal static IObservable<T> ToObservableWithIdMultiple<T, TEnd>(
            this IObservable<object> source, Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
                where T : IHasRequestId where TEnd : T
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<T>()
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<T>(
                        onNext: m =>
                        {
                            if (m is not TEnd)
                            {
                                observer.OnNext(m);
                                return;
                            }
                            cancelable = false;
                            observer.OnCompleted();
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                if (cancelable == null)
                    startRequest(requestId);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest?.Invoke(requestId);
                    subscription.Dispose();
                });
            });
        }


        // Continuous results: Tick, MarketDepth, RealtimeBar
        internal static IObservable<T> ToObservableWithIdContinuous<T>(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int> stopRequest)
                where T : IHasRequestId
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<T>()
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(observer);

                if (cancelable == null)
                    startRequest(requestId);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest.Invoke(requestId);
                    subscription.Dispose();
                });
            });
        }
    }
}
