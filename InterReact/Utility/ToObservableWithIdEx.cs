﻿using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact
{
    // For requests that use RequestId.
    public static partial class Extensions
    {
        // Single result: HistoricalData, FundamentalData, ScannerData, SymbolSamples.
        internal static IObservable<IHasRequestId> ToObservableWithIdSingle(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int>? stopRequest = null)
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int id = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == id)
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
                    startRequest(id);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest?.Invoke(id);
                    subscription.Dispose();
                });
            });
        }


        // Multiple results: TickSnapshot, ContractDetails, Executions.
        internal static IObservable<IHasRequestId> ToObservableWithIdMultiple<TEnd>(
            this IObservable<object> source, Func<int> getNextId, Action<int> startRequest)
                where TEnd: IHasRequestId
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int id = getNextId();

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == id)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: m =>
                        {
                            if (m is TEnd)
                                observer.OnCompleted();
                            else
                                observer.OnNext(m);
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                startRequest(id);

                return subscription;
            });
        }


        // Continuous results: AccountSummary, Tick, MarketDepth, RealtimeBar.
        internal static IObservable<IHasRequestId> ToObservableWithIdContinuous(this IObservable<object> source,
            Func<int> getNextId, Action<int> startRequest, Action<int> stopRequest)
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int id = getNextId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<IHasRequestId>()
                    .Where(m => m.RequestId == id)
                    .Finally(() => cancelable = false)
                    .SubscribeSafe(observer);

                if (cancelable == null)
                    startRequest(id);
                if (cancelable == null)
                    cancelable = true;

                return Disposable.Create(() =>
                {
                    if (cancelable == true)
                        stopRequest.Invoke(id);
                    subscription.Dispose();
                });
            });
        }
    }
}
