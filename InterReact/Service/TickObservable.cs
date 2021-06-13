using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates a connectable observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, 
        /// release all subscriptions and clear the cache.
        /// Returns: 
        /// </summary>
        public IObservable<ITick> CreateTickObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false,
            IList<Tag>? options = null)
        {
            return Response.
                ToObservableWithIdContinuous<ITick>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId));
        }


        /// <summary>
        /// Creates a connectable observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, 
        /// release all subscriptions and clear the cache.
        /// </summary>
        public IConnectableObservable<ITick> CreateTickConnectableObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false,
            IList<Tag>? options = null)
        {
            return Response.
                ToObservableWithIdContinuous<ITick>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId))
                //.ToCacheSource(tick => tick.TickType);
                .ToCacheSource(tick => tick);
        }
    }

    public static partial class Extensions
    {
        public static IObservable<TickPrice> OfTypeTickPrice(this IObservable<ITick> source) => source.OfType<TickPrice>();
        public static IObservable<TickSize> OfTypeTickSize(this IObservable<ITick> source) => source.OfType<TickSize>();
        public static IObservable<TickString> OfTypeTickString(this IObservable<ITick> source) => source.OfType<TickString>();
        public static IObservable<TickTime> OfTypeTickTime(this IObservable<ITick> source) => source.OfType<TickTime>();
        public static IObservable<TickRealtimeVolume> OfTypeTickRealtimeVolume(this IObservable<ITick> source) => source.OfType<TickRealtimeVolume>();
        public static IObservable<TickGeneric> OfTypeTickGeneric(this IObservable<ITick> source) => source.OfType<TickGeneric>();
        public static IObservable<TickExchangeForPhysical> OfTypeTickExchangeForPhysical(this IObservable<ITick> source) => source.OfType<TickExchangeForPhysical>();
        public static IObservable<TickOptionComputation> OfTypeTickOptionComputation(this IObservable<ITick> source) => source.OfType<TickOptionComputation>();
        public static IObservable<TickHalted> OfTypeTickHalted(this IObservable<ITick> source) => source.OfType<TickHalted>();

        public static IObservable<T> UndelayTicks<T>(this IObservable<T> source)
        {
            return source.Do(x =>
            {
                if (x is Tick tick)
                    tick.Undelay();
            });
        }
    }

}
