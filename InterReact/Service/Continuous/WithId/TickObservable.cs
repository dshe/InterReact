using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits a snapshot of market ticks, then completes.
        /// Tick types may be selected by using the OfTickType extension method.
        /// </summary>
        public IObservable<ITick> CreateTickSnapshotObservable(Contract contract)
        {
            return Response
                .ToObservableMultipleWithId<SnapshotEndTick>(
                    Request.GetNextId,
                    id => Request.RequestMarketData(id, contract, genericTickTypes: null, isSnapshot: true))
                .Cast<ITick>()
                .ShareSource();
        }

        /// <summary>
        /// Creates an observable which continually emits market data ticks for the specified contract.
        /// Use CreateTickObservable(...).Publish()[.RefCount()] to share the subscription.
        /// Use CreateTickObservable(...).ShareSourceCache(Services.GetTickCacheKey)
        /// to cache the latest values for replay to new subscribers.
        /// Tick types may be selected by using the OfTickType extension method.
        /// </summary>
        public IObservable<ITick> CreateTickObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false, IList<Tag>? options = null)
        {
            return Response
                .ToObservableContinuousWithId(
                    Request.GetNextId,
                    id => Request.RequestMarketData(id, contract, genericTickTypes, marketDataOff, false, options),
                    id => Request.CancelMarketData(id))
            .Cast<ITick>();
        }

        public static string GetTickCacheKey(ITick itick)
        {
            return itick switch
            {
                PriceTick tp => $"{tp.TickType}+{tp.TickAttrib}",
                SizeTick ts => $"{ts.TickType}",
                Alert alert => $"{alert.Code}+{alert.Message}",
                _ => throw new ArgumentException($"Unhandled type: {itick.GetType()}.")
            };
        }
    }

    public class TickSelector
    {
        private readonly IObservable<ITick> Source;
        public TickSelector(IObservable<ITick> source)
        {
            Source = source;
        }
        public IObservable<PriceTick> TickPrice => Source.OfType<PriceTick>();
        public IObservable<SizeTick> TickSize => Source.OfType<SizeTick>();
        public IObservable<StringTick> TickString => Source.OfType<StringTick>();
        public IObservable<TimeTick> TickTime => Source.OfType<TimeTick>();
        public IObservable<RealtimeVolumeTick> TickRealtimeVolume => Source.OfType<RealtimeVolumeTick>();
        public IObservable<GenericTick> TickGeneric => Source.OfType<GenericTick>();
        public IObservable<ExchangeForPhysicalTick> TickExchangeForPhysical => Source.OfType<ExchangeForPhysicalTick>();
        public IObservable<OptionComputationTick> TickOptionComputation => Source.OfType<OptionComputationTick>();
        public IObservable<HaltedTick> TickHalted => Source.OfType<HaltedTick>();
        public IObservable<MarketDataTypeTick> TickMarketDataType => Source.OfType<MarketDataTypeTick>();
        public IObservable<Alert> Alert => Source.OfType<Alert>();
    }

    public static partial class Extensions
    {
        public static IObservable<T> OfTickType<T>(this IObservable<ITick> source, Func<TickSelector, IObservable<T>> selector) =>
           selector(new TickSelector(source));

        public static IObservable<ITick> UndelayTicks(this IObservable<ITick> source) =>
            source.Do(x =>
            {
                if (x is BaseTick tick)
                    tick.Undelay();
            });
    }
}
