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
        internal static IObservable<IHasRequestId> ToObservableWithIdSingle(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
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
        internal static IObservable<IHasRequestId> ToObservableWithIdMultiple<TEnd>(
            this IObservable<object> source, Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
                where TEnd: IHasRequestId
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
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
        internal static IObservable<IHasRequestId> ToObservableWithIdContinuous(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int> stopRequest)
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == requestId)
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
