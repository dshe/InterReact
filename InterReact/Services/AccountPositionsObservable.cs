using Stringification;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits Position objects for all accounts.
    /// All positions are sent initially, and then only updates. 
    /// The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<Position> PositionsObservable { get; }

    private IObservable<Position> CreatePositionsObservable()
    {
        return Response
            .OfType<Position>()
            .ToObservableContinuous(
                Request.RequestPositions,
                Request.CancelPositions)
            .CacheSource(p => $"{p.Account}: {p.Contract.Stringify()}");
    }
}
