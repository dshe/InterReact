using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits a snapshot of market data ticks, then completes.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<ITick> CreateTickSnapshotObservable(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
    {
        return Response
            .ToObservableMultipleWithId<SnapshotEndTick>(
                Request.GetNextId,
                id => Request.RequestMarketData(id, contract, genericTickTypes, true, isRegulatorySnapshot, options))
            .Cast<ITick>()
            .ShareSource();
    }

    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// Use CreateTickObservable(...).Publish()[.RefCount() | .AutoConnect()] to share the subscription.
    /// Or use CreateTickObservable(...).CacheSource(InterReact.Services.GetTickCacheKey)
    /// to cache the latest values for replay to new subscribers.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<ITick> CreateTickObservable(Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null, IEnumerable<Tag>? options = null)
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestMarketData(id, contract, genericTickTypes, false, false, options),
                id => Request.CancelMarketData(id))
            .Cast<ITick>();
    }

    public static string GetTickCacheKey(IHasRequestId t) => t is Tick tick ? tick.TickType.ToStringFast() : "";
}

public class TickClassSelector
{
    private readonly IObservable<ITick> Source;
    public TickClassSelector(IObservable<ITick> source) => Source = source;
    public IObservable<PriceTick> PriceTick => Source.OfType<PriceTick>();
    public IObservable<SizeTick> SizeTick => Source.OfType<SizeTick>();
    public IObservable<StringTick> StringTick => Source.OfType<StringTick>();
    public IObservable<TimeTick> TimeTick => Source.OfType<TimeTick>();
    public IObservable<RealtimeVolumeTick> RealtimeVolumeTick => Source.OfType<RealtimeVolumeTick>();
    public IObservable<GenericTick> GenericTick => Source.OfType<GenericTick>();
    public IObservable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => Source.OfType<ExchangeForPhysicalTick>();
    public IObservable<OptionComputationTick> OptionComputationTick => Source.OfType<OptionComputationTick>();
    public IObservable<HaltedTick> HaltedTick => Source.OfType<HaltedTick>();
    public IObservable<MarketDataTypeTick> MarketDataTypeTick => Source.OfType<MarketDataTypeTick>();
    public IObservable<ReqParamsTick> ReqParamsTick => Source.OfType<ReqParamsTick>();
    public IObservable<Alert> Alert => Source.OfType<Alert>();
}

public static partial class Ext
{
    public static IObservable<T> OfTickClass<T>(this IObservable<ITick> source, Func<TickClassSelector, IObservable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return selector(new TickClassSelector(source));
    }

    // If tick is delayed, substitute the corresponding 
    public static IObservable<ITick> UndelayTicks(this IObservable<ITick> source) =>
        source.Do(x =>
        {
            if (x is Tick tick)
                tick.Undelay();
        });
}
