namespace InterReact;

// This extension method provides an alternative to source.OfType<T>, with intellisense support.
// So that, for example, source.OfType<PriceTick>() can be replaced with OfTickClass(selector => selector.PriceTick).
public static partial class Xtensions
{
    extension<T>(IEnumerable<IHasRequestId> source)
    {
        public IEnumerable<T> OfTickClass(Func<TickEnumerableSelector, IEnumerable<T>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return selector(new TickEnumerableSelector(source));
        }
    }

    extension<T>(IObservable<IHasRequestId> source)
    {
        public IObservable<T> OfTickClass(Func<TickObservableSelector, IObservable<T>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return selector(new TickObservableSelector(source));
        }
    }
}

public sealed class TickEnumerableSelector(IEnumerable<IHasRequestId> source)
{
    public IEnumerable<PriceTick> PriceTick => source.OfType<PriceTick>();
    public IEnumerable<SizeTick> SizeTick => source.OfType<SizeTick>();
    public IEnumerable<StringTick> StringTick => source.OfType<StringTick>();
    public IEnumerable<TimeTick> TimeTick => source.OfType<TimeTick>();
    public IEnumerable<RealtimeVolumeTick> RealtimeVolumeTick => source.OfType<RealtimeVolumeTick>();
    public IEnumerable<GenericTick> GenericTick => source.OfType<GenericTick>();
    public IEnumerable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => source.OfType<ExchangeForPhysicalTick>();
    public IEnumerable<OptionComputationTick> OptionComputationTick => source.OfType<OptionComputationTick>();
    public IEnumerable<Alert> Alert => source.OfType<Alert>();
}

public sealed class TickObservableSelector(IObservable<IHasRequestId> source)
{
    public IObservable<PriceTick> PriceTick => source.OfType<PriceTick>();
    public IObservable<SizeTick> SizeTick => source.OfType<SizeTick>();
    public IObservable<StringTick> StringTick => source.OfType<StringTick>();
    public IObservable<TimeTick> TimeTick => source.OfType<TimeTick>();
    public IObservable<RealtimeVolumeTick> RealtimeVolumeTick => source.OfType<RealtimeVolumeTick>();
    public IObservable<GenericTick> GenericTick => source.OfType<GenericTick>();
    public IObservable<ExchangeForPhysicalTick> ExchangeForPhysicalTick => source.OfType<ExchangeForPhysicalTick>();
    public IObservable<OptionComputationTick> OptionComputationTick => source.OfType<OptionComputationTick>();
    public IObservable<Alert> Alert => source.OfType<Alert>();
}
