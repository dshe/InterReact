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
    public IObservable<object> CreateTickObservable(Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null, params Tag[] options)
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestMarketData(id, contract, genericTickTypes, false, false, options),
                Request.CancelMarketData);
    }

    public static string GetTickCacheKey(IHasRequestId t) => t is ITick tick ? tick.TickType.ToString() : "";
}

public sealed class TickClassSelector
{
    private readonly IObservable<object> Source;
    public TickClassSelector(IObservable<object> source) => Source = source;
    public IObservable<PriceTick> PriceTick => Source.OfType<PriceTick>();
    public IObservable<SizeTick> SizeTick => Source.OfType<SizeTick>();
    public IObservable<StringTick> StringTick => Source.OfType<StringTick>();
    public IObservable<TimeTick> TimeTick => Source.OfType<TimeTick>();
    public IObservable<RealtimeVolumeTick> RealtimeVolumeTick => Source.OfType<RealtimeVolumeTick>();
    public IObservable<GenericTick> GenericTick => Source.OfType<GenericTick>();
    public IObservable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => Source.OfType<ExchangeForPhysicalTick>();
    public IObservable<OptionComputationTick> OptionComputationTick => Source.OfType<OptionComputationTick>();
    public IObservable<AlertMessage> Alert => Source.OfType<AlertMessage>();
}

public static partial class Extension
{
    public static IObservable<T> OfTickClass<T>(this IObservable<object> source, Func<TickClassSelector, IObservable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return selector(new TickClassSelector(source));
    }
}
