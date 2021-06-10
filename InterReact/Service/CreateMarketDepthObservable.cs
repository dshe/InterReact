using System;
using System.Collections.Generic;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits market depth for the specified contract.
        /// </summary>
        public IObservable<MarketDepth> CreateMarketDepthObservable(Contract contract, int rows = 3, IList<Tag>? options = null)
        {
            if (rows < 1)
                throw new ArgumentException("rows < 1", nameof(rows));

            return Response.ToObservableWithIdContinuous<MarketDepth>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketDepth(requestId, contract, rows, options),
                    requestId => Request.CancelMarketDepth(requestId));
        }
    }
}
