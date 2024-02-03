namespace InterReact;

// This extension method provides an alternative to source.OfType<T, with intellisense support.
// For example, source.OfType<PriceTick>() can be replaced with OfTickClass(selector => selector.PriceTick).
public static partial class Extension
{
    public static IObservable<T> OfTickClass<T>(
        this IObservable<IHasRequestId> source,
        Func<TickClassSelector, IObservable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return selector(new TickClassSelector(source));
    }
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
    public IObservable<AlertMessage> Alert => Source.OfType<AlertMessage>();
}
