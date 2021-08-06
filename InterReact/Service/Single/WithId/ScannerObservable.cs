using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits scanner results, then completes.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<ScannerData, Alert>> CreateScannerObservable(
            ScannerSubscription subscription, IList<Tag>? subscriptionOptions = null)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            return Response
                .ToObservableSingleWithId(
                    Request.GetNextId,
                    id => Request.RequestScannerSubscription(
                        id,
                        subscription,
                        subscriptionOptions),
                    Request.CancelScannerSubscription)
                .Select(x => new Union<ScannerData, Alert>(x));
        }
    }
}
