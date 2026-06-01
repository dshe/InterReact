namespace InterReact;

// This extension method provides an alternative to source.OfType<T>, with intellisense support.
// So that, for example, source.OfType<PriceTick>() can be replaced with OfTickClass(selector => selector.PriceTick).
public static partial class Extension
{
    public static IEnumerable<T> OfTickClass<T>(
        this IEnumerable<IHasRequestId> source,
        Func<TickEnumerableSelector, IEnumerable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return selector(new TickEnumerableSelector(source));
    }

    public static IObservable<T> OfTickClass<T>(
        this IObservable<IHasRequestId> source,
        Func<TickObservableSelector, IObservable<T>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return selector(new TickObservableSelector(source));
    }
}

public sealed class TickEnumerableSelector(IEnumerable<IHasRequestId> source)
{
    private readonly IEnumerable<IHasRequestId> _source = source;
    public IEnumerable<PriceTick> PriceTick => _source.OfType<PriceTick>();
    public IEnumerable<SizeTick> SizeTick => _source.OfType<SizeTick>();
    public IEnumerable<StringTick> StringTick => _source.OfType<StringTick>();
    public IEnumerable<TimeTick> TimeTick => _source.OfType<TimeTick>();
    public IEnumerable<RealtimeVolumeTick> RealtimeVolumeTick => _source.OfType<RealtimeVolumeTick>();
    public IEnumerable<GenericTick> GenericTick => _source.OfType<GenericTick>();
    public IEnumerable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => _source.OfType<ExchangeForPhysicalTick>();
    public IEnumerable<OptionComputationTick> OptionComputationTick => _source.OfType<OptionComputationTick>();
    public IEnumerable<Alert> Alert => _source.OfType<Alert>();
}

public sealed class TickObservableSelector(IObservable<IHasRequestId> source)
{
    private readonly IObservable<IHasRequestId> _source = source;
    public IObservable<PriceTick> PriceTick => _source.OfType<PriceTick>();
    public IObservable<SizeTick> SizeTick => _source.OfType<SizeTick>();
    public IObservable<StringTick> StringTick => _source.OfType<StringTick>();
    public IObservable<TimeTick> TimeTick => _source.OfType<TimeTick>();
    public IObservable<RealtimeVolumeTick> RealtimeVolumeTick => _source.OfType<RealtimeVolumeTick>();
    public IObservable<GenericTick> GenericTick => _source.OfType<GenericTick>();
    public IObservable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => _source.OfType<ExchangeForPhysicalTick>();
    public IObservable<OptionComputationTick> OptionComputationTick => _source.OfType<OptionComputationTick>();
    public IObservable<Alert> Alert => _source.OfType<Alert>();
}
