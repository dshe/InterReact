using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.Messages;
using System.Diagnostics;

#nullable enable

namespace InterReact.Utility.Rx
{
    internal static class ObservableFactoryEx
    {
        // For requests that do not use requestId.
        // Single result(end=null): CurrentTime, ScannerParameters
        // Multiple results (end=false=>true): AccountPositions, OpenOrders
        // Continuous results (end=false): AccountUpdate, NewsBulletins
        // End type: not type T
        internal static IObservable<T> ToObservable<T>(
            this IObservable<object> source, Action subscribe, Action? unsubscribe = null, Func<object, bool>? end = null)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (subscribe == null)
                throw new ArgumentNullException(nameof(subscribe));

            return Observable.Create<T>(observer =>
            {
                bool? cancelable = null;

                var subscription = source
                    .Finally(() => cancelable = false)
                    .Subscribe(
                        onNext: m =>
                        {
                            var theEnd = end != null && end(m);
                            if (m is T t && (end == null || !theEnd))
                            {
                                observer.OnNext(t);
                            }
                            if (end == null || theEnd)
                            {
                                cancelable = false;
                                observer.OnCompleted();
                            }
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted);

                if (cancelable == null)
                    subscribe();
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                    {
                        try
                        {
                            unsubscribe?.Invoke();
                        }
                        catch (Exception e)
                        {   // ignored
                            Debug.WriteLine("Unexpected: " + e.ToString());
                        }
                    }
                    subscription.Dispose();
                });
            });
        }

        // For requests that use RequestId.
        // Single result (end=null): HistoricalData, FundamentalData, ScannerData
        // Multiple results (end=false=>true): TickSnapshot, ContractData, AccountSummary, Executions
        // Continuous results (end=false): MarketDepth, Tick, RealtimeBar
        // End type: not type T
        internal static IObservable<T> ToObservable<T>(
            this IObservable<object> source, Func<int> nextId, Action<int> subscribe, 
                Action<int>? unsubscribe = null, Func<IHasRequestId, bool>? end = null)
            where T : class, IHasRequestId
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (nextId == null)
                throw new ArgumentNullException(nameof(nextId));
            if (subscribe == null)
                throw new ArgumentNullException(nameof(subscribe));

            return Observable.Create<T>(observer =>
            {
                var requestId = nextId();
                bool? cancelable = null;

                var subscription = source
                    .OfType<IHasRequestId>()
                    .Where(x => x.RequestId == requestId)
                    .Finally(() => cancelable = false)
                    .Subscribe(
                        onNext: m =>
                        {
                            if (m is Alert alert)
                            {
                                cancelable = false;
                                observer.OnError(alert); // ???
                                //observer.OnNext(alert);
                            }
                            else if (end != null && end(m))
                            {
                                cancelable = false;
                                observer.OnCompleted();
                            }
                            else if (m is T t)
                            {
                                observer.OnNext(t);
                                if (end == null)
                                {
                                    cancelable = false;
                                    observer.OnCompleted();
                                }
                            }
                            else
                                throw new InvalidOperationException($"Invalid type: {m.GetType()}.");
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted);

                if (cancelable == null)
                    subscribe(requestId);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                    {
                        try
                        {
                            unsubscribe?.Invoke(requestId);
                        }
                        catch (Exception e)
                        {   // ignored
                            Debug.WriteLine("Unexpected: " + e.ToString());
                        }
                    }
                    subscription.Dispose();
                });
            });
        }
    }
}
