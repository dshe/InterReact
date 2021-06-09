using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates a ConnectableOservable which emits market depth for the specified contract.
        /// Can be transformed into observable collections using ToObservableCollections().
        /// Don't forget to Connect().
        /// </summary>
        public IConnectableObservable<MarketDepth> CreateMarketDepthObservable(Contract contract, int rows = 3, IList<Tag>? options = null)
        {
            if (rows < 1)
                throw new ArgumentException("rows < 1", nameof(rows));

            return Response.ToObservableWithIdContinuous<MarketDepth>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketDepth(requestId, contract, rows, options),
                    requestId => Request.CancelMarketDepth(requestId))
               .Publish();
        }
    }
}
