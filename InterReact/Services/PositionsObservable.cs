using Stringification;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits Position objects for all accounts.
    /// All positions are sent initially, and then only updates as positions change. 
    /// PositionEnd is emitted after the initial values for each account have been emitted.
    /// Multiple subscribers are supported. The latest values are cached for replay to new subscribers.
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

    private static string GetPositionsCacheKey(object o)
    {
        return o switch
        {
            Position p => $"{p.Account}+{p.Contract.Stringify()}",
            PositionEnd => "PositionEnd",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}
