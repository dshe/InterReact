using Stringification;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits Position objects for all accounts.
    /// Objects: Position, PositionsEnd.
    /// All positions are sent initially, and then only updates as positions change. 
    /// PositionEnd is emitted after the initial values for each account have been emitted.
    /// The latest values are cached for replay to new subscribers.
    /// Multiple subscribers are supported. 
    /// </summary>
    public IObservable<object> PositionsObservable { get; }

    private IObservable<object> CreatePositionsObservable()
    {
        return Response
            .Where(x => x is Position || x is PositionEnd)
            .ToObservableContinuous(
                Request.RequestPositions,
                Request.CancelPositions)
            .CacheSource(GetPositionsCacheKey);
    }

    private static string GetPositionsCacheKey(object m)
    {
        return m switch
        {
            Position p => $"{p.Account}+{p.Contract.Stringify()}",
            PositionEnd => "PositionEnd",
            _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
        };
    }
}
