using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits scanner results, then completes.
        /// returns: ScannerData, Alert
        /// </summary>
        public IObservable<IHasRequestId> CreateScannerObservable(
            ScannerSubscription subscription, IList<Tag>? subscriptionOptions = null)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            return Response
                .ToObservableWithIdSingle<IScannerData>(
                    Request.GetNextId,
                    requestId => Request.RequestScannerSubscription(
                        requestId,
                        subscription,
                        subscriptionOptions),
                    Request.CancelScannerSubscription)
                .ToShareSource();
        }
    }

    public static partial class Extensions
    {
        public static IObservable<ScannerData> OfTypeScannerData(this IObservable<IScannerData> source) =>
            source.OfType<ScannerData>();
    }
}
