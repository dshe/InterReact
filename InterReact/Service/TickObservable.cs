using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Publish() Connect() to start receiving updates.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, 
        /// release all subscriptions and clear the cache.
        /// Returns: 
        /// </summary>
        //
        public IObservable<ITick> CreateTickObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false, IList<Tag>? options = null)
        {
            return Response
                .ToObservableWithIdContinuous(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId))
            .Cast<ITick>();
        }

        /// <summary>
        /// Creates an observable which emits a snapshot of market ticks, then completes.
        /// </summary>
        public IObservable<ITick> CreateTickSnapshotObservable(Contract contract)
        {
            return Response
                .ToObservableWithIdMultiple<TickSnapshotEnd>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes: null, isSnapshot: true))
                .Cast<ITick>()
                .ToShareSource();
        }
    }

    public class TickSelector
    {
        private readonly IObservable<ITick> Source;
        public TickSelector(IObservable<ITick> source) => Source = source;
        public IObservable<TickPrice> TickPrice => Source.OfType<TickPrice>();
        public IObservable<TickSize> TickSize => Source.OfType<TickSize>();
        public IObservable<TickString> TickString => Source.OfType<TickString>();
        public IObservable<TickTime> TickTime => Source.OfType<TickTime>();
        public IObservable<TickRealtimeVolume> TickRealtimeVolume => Source.OfType<TickRealtimeVolume>();
        public IObservable<TickGeneric> TickGeneric => Source.OfType<TickGeneric>();
        public IObservable<TickExchangeForPhysical> TickExchangeForPhysical => Source.OfType<TickExchangeForPhysical>();
        public IObservable<TickOptionComputation> TickOptionComputation => Source.OfType<TickOptionComputation>();
        public IObservable<TickHalted> TickHalted => Source.OfType<TickHalted>();
        public IObservable<TickMarketDataType> TickMarketDataType => Source.OfType<TickMarketDataType>();
        public IObservable<Alert> Alert => Source.OfType<Alert>();
    }

    public static partial class Extensions
    {
        public static IObservable<T> OfType<T>(this IObservable<ITick> source, Func<TickSelector, IObservable<T>> selector) =>
           selector(new TickSelector(source));

        public static IObservable<ITick> UndelayTicks(this IObservable<ITick> source) =>
            source.Do(x =>
            {
                if (x is TickBase tick)
                    tick.Undelay();
            });
    }
}
