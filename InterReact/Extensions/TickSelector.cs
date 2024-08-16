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
    private readonly IEnumerable<IHasRequestId> Source = source;
    public IEnumerable<PriceTick> PriceTick => Source.OfType<PriceTick>();
    public IEnumerable<SizeTick> SizeTick => Source.OfType<SizeTick>();
    public IEnumerable<StringTick> StringTick => Source.OfType<StringTick>();
    public IEnumerable<TimeTick> TimeTick => Source.OfType<TimeTick>();
    public IEnumerable<RealtimeVolumeTick> RealtimeVolumeTick => Source.OfType<RealtimeVolumeTick>();
    public IEnumerable<GenericTick> GenericTick => Source.OfType<GenericTick>();
    public IEnumerable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => Source.OfType<ExchangeForPhysicalTick>();
    public IEnumerable<OptionComputationTick> OptionComputationTick => Source.OfType<OptionComputationTick>();
    public IEnumerable<AlertMessage> Alert => Source.OfType<AlertMessage>();
}

public sealed class TickObservableSelector(IObservable<IHasRequestId> source)
{
    private readonly IObservable<IHasRequestId> Source = source;
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
