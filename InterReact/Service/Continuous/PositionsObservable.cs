using Stringification;
using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits postions for all accounts.
        /// All positions are sent initially, and then only updates as positions change. 
        /// PositionEnd is emitted after the initial values for each account have been emitted.
        /// Use CreatePositionsObservable().Publish()[.RefCount()] to share the subscription.
        /// Use CreatePositionsObservable().ShareSourceCache(Services.GetPositionsCacheKey)
        /// to cache the latest values for replay to new subscribers.
        /// </summary>
        public IObservable<Union<Position, PositionEnd>> CreatePositionsObservable()
        {
            return Response
                .Where(x => x is Position || x is PositionEnd)
                .ToObservableContinuous(
                    Request.RequestPositions,
                    Request.CancelPositions)
                .Select(x => new Union<Position, PositionEnd>(x));
        }

        public static string GetPositionsCacheKey(Union<Position, PositionEnd> union)
        {
            return union.Source switch
            {
                Position p => $"{p.Account}+{p.Contract.Stringify()}",
                PositionEnd => "PositionEnd",
                _ => throw new ArgumentException($"Unhandled type: {union.Source.GetType()}.")
            };
        }

    }
}
