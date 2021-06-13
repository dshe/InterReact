using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits market depth for the specified contract.
        /// </summary>
        public IObservable<IMarketDepth> CreateMarketDepthObservable(Contract contract, int rows = 3, IList<Tag>? options = null)
        {
            if (rows < 1)
                throw new ArgumentException("rows < 1", nameof(rows));

            return Response.ToObservableWithIdContinuous<IMarketDepth>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketDepth(requestId, contract, rows, options),
                    requestId => Request.CancelMarketDepth(requestId));
        }
    }

    public static partial class Extensions
    {
        public static IObservable<MarketDepth> OfTypeMarketDepth(this IObservable<IMarketDepth> source) => source.OfType<MarketDepth>();
    }

}
