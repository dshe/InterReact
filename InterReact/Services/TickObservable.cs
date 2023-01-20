using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// Use CreateTickObservable(...).Publish()[.RefCount() | .AutoConnect()] to share the subscription.
    /// Or use CreateTickObservable(...).CacheSource(InterReact.Services.GetTickCacheKey)
    /// to cache the latest values for replay to new subscribers.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateTickObservable(Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null, IEnumerable<Tag>? options = null)
    {
        return Response
            .ToObservableContinuousWithRequestId(
                Request.GetNextId,
                requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, false, false, options),
                requestId => Request.CancelMarketData(requestId))
            .Cast<IHasRequestId>();
    }

    public static string GetTickCacheKey(IHasRequestId t) => t is Tick tick ? tick.TickType.ToStringFast() : "";
}

public sealed class TickClassSelector
{
    private readonly IObservable<IHasRequestId> Source;
    public TickClassSelector(IObservable<IHasRequestId> source) => Source = source;
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

public static partial class Extensionz
{
    public static IObservable<T> OfTickClass<T>(this IObservable<IHasRequestId> source, Func<TickClassSelector, IObservable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return selector(new TickClassSelector(source));
    }

    // If tick is delayed, substitute the corresponding 
    public static IObservable<IHasRequestId> UndelayTicks(this IObservable<IHasRequestId> source) =>
        source.Do(x =>
        {
            if (x is Tick tick)
                tick.Undelay();
        });
}
