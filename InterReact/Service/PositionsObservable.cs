using Stringification;
using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits postions for all accounts.
        /// All positions are sent initially, and then only updates as positions change. 
        /// PositionEnd is emitted after the initial values for each account have been emitted.
        /// </summary>
        public IObservable<Union<Position, PositionEnd>> PositionsObservable { get; }

        private IObservable<Union<Position, PositionEnd>> CreateAccountPositionsObservable()
        {
            return Response
                .Where(x => x is Position || x is PositionEnd)
                .ToObservableContinuous(
                    Request.RequestPositions,
                    Request.CancelPositions)
                .Select(x => new Union<Position, PositionEnd>(x))
                .ShareSourceCache(GetCacheKey);

            // local
            static string GetCacheKey(Union<Position, PositionEnd> union)
            {
                return union.Source switch
                {
                    Position p => $"{p.Account}+{p.Contract.Stringify()}",
                    PositionEnd => "PositionEnd",
                    _ => ""
                };
            }


        }
    }
}
