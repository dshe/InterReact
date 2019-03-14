using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility.Rx;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates a connectable observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() the value returned from Connect() to disconnect from the source, release all subscriptions and clear the cache.
        /// </summary>
        public IConnectableObservable<ITick> TickConnectableObservable(Contract contract,
            IEnumerable<GenericTickType>? genericTickTypes = null, bool marketDataOff = false, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            return Response.ToObservable<ITick>(
                    Request.NextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId),
                    m => false)
                .ToCacheSource(tick => tick.TickType);
        }

        /// <summary>
        /// Creates an observable which emits a snapshot of market ticks, then completes.
        /// </summary>
        public IObservable<ITick> TickSnapshotObservable(Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            return Response.ToObservable<ITick>(
                    Request.NextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes:null, isSnapshot: true),
                    null, m => m is TickSnapshotEnd)
                .ToShareSource();
        }

        /// <summary>
        /// Creates a ConnectableOservable which emits market depth for the specified contract.
        /// Can be transformed into observable collections using ToObservableCollections().
        /// Don't forget to Connect().
        /// </summary>
        public IConnectableObservable<MarketDepth> MarketDepthObservable(Contract contract, int rows = 3, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            if (rows < 1)
                throw new ArgumentException(nameof(rows));

            return Response.ToObservable<MarketDepth>(
                    Request.NextId,
                    requestId => Request.RequestMarketDepth(requestId, contract, rows, options),
                    requestId => Request.CancelMarketDepth(requestId),
                    m => false)
               .Publish();
        }

        /// <summary>
        /// Creates an observable which continually emits 5 second bars.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<RealtimeBar> RealtimeBarObservable(Contract contract,
            RealtimeBarType? whatToShow = null, bool regularTradingHours = true, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            whatToShow = whatToShow ?? RealtimeBarType.Trades;

            return Response.ToObservable<RealtimeBar>(
                    Request.NextId,
                    requestId => Request.RequestRealTimeBars(requestId, contract, whatToShow, regularTradingHours, options),
                    requestId => Request.CancelRealTimeBars(requestId),
                    m => false)
                .Publish().RefCount();
        }


    }

}
