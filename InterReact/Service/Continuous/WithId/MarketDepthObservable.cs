using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which emits market depth for the specified contract.
        /// Use CreateMarketDepthObservable(...).Publish()[.RefCount() | .AutoConnect()] to share the subscription.
        /// </summary>
        public IObservable<Union<MarketDepth, Alert>> CreateMarketDepthObservable(Contract contract, int rows = 3, IEnumerable<Tag>? options = null)
        {
            if (rows < 1)
                throw new ArgumentException("rows < 1", nameof(rows));

            return Response.ToObservableContinuousWithId(
                    Request.GetNextId,
                    id => Request.RequestMarketDepth(id, contract, rows, options),
                    id => Request.CancelMarketDepth(id))
                .Select(x => new Union<MarketDepth, Alert>(x));
        }
    }
}
