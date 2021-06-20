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
        public IObservable<Union<MarketDepth, Alert>> CreateMarketDepthObservable(Contract contract, int rows = 3, IList<Tag>? options = null)
        {
            if (rows < 1)
                throw new ArgumentException("rows < 1", nameof(rows));

            return Response.ToObservableWithIdContinuous(
                    Request.GetNextId,
                    id => Request.RequestMarketDepth(id, contract, rows, options),
                    id => Request.CancelMarketDepth(id))
                .Select(x => new Union<MarketDepth, Alert>(x));
        }
    }
}
