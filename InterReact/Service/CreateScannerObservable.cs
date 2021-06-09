using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits scanner results, then completes.
        /// </summary>
        public IObservable<List<ScannerDataItem>> CreateScannerObservable(
            ScannerSubscription subscription, IList<Tag>? subscriptionOptions = null)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            return Response
                .ToObservableWithIdSingle<ScannerData>(
                    Request.GetNextId,
                    requestId => Request.RequestScannerSubscription(
                        requestId,
                        subscription,
                        subscriptionOptions),
                    Request.CancelScannerSubscription)
                .Select(m => m.Items)
                .ToShareSource();
        }
    }
}
