using System;
using System.IO;
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
                where T : class, IHasRequestId
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: m =>
                        {
                            cancelable = false;
                            if (m is T t1)
                            {
                                observer.OnNext(t1);
                                observer.OnCompleted();
                                return;
                            }
                            if (m is Alert alert)
                            {
                                T? t2 = Utilities.TryConstructSubclassTaking<T>(alert);
                                if (t2 != null)
                                {
                                    observer.OnNext(t2);
                                    observer.OnCompleted();
                                    return;
                                }
                                observer.OnError(alert);
                                return;
                            }
                            throw new InvalidDataException($"Invalid type: {m.GetType()}.");
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
        internal static IObservable<T> ToObservableWithIdMultiple<T, TEnd>(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
                where T : class, IHasRequestId
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: m =>
                        {
                            if (m is T t1)
                            {
                                observer.OnNext(t1);
                                return;
                            }
                            if (m is Alert alert)
                            {
                                T? t2 = Utilities.TryConstructSubclassTaking<T>(alert);
                                if (t2 != null)
                                {
                                    observer.OnNext(t2);
                                    return;
                                }
                                cancelable = false;
                                observer.OnError(alert);
                                return;
                            }
                            if (m is TEnd)
                            {
                                cancelable = false;
                                observer.OnCompleted();
                                return;
                            }
                            throw new InvalidDataException($"Invalid type: {m.GetType()}.");
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
                where T : class, IHasRequestId
        {
            return Observable.Create<T>(observer =>
            {
                int requestId = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .WithRequestId(requestId)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: m =>
                        {
                            if (m is T t1)
                            {
                                observer.OnNext(t1);
                                return;
                            }
                            if (m is Alert alert)
                            {
                                T? t2 = Utilities.TryConstructSubclassTaking<T>(alert);
                                if (t2 != null)
                                {
                                    observer.OnNext(t2);
                                    return;
                                }
                                cancelable = false;
                                observer.OnError(alert);
                                return;
                            }
                            throw new InvalidDataException($"Invalid type: {m.GetType()}.");
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
                        stopRequest.Invoke(requestId);
                    subscription.Dispose();
                });
            });
        }
    }
}
