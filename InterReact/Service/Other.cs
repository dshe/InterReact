using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.Utility.Rx;
using NodaTime;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which gets the current time from TWS. Precision: seconds.
        /// </summary>
        public IObservable<Instant> CurrentTimeObservable =>
            Response
            .ToObservable<CurrentTime>(() => Request.RequestCurrentTime())
            .Select(m => m.Time)
            .ToShareSource();

        /// <summary>
        /// An observable which continually emits news bulletins.
        /// News bulletins inform of important exchange disruptions.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<NewsBulletin> NewsBulletinsObservable =>
            Response
            .ToObservable<NewsBulletin>(() => Request.RequestNewsBulletins(), Request.CancelNewsBulletins, m => false)
            .Publish()
            .RefCount();

        /// <summary>
        /// An observable which, upon subscription, emits scanner parameters, then completes.
        /// </summary>
        public IObservable<string> ScannerParametersObservable =>
            Response
            .ToObservable<ScannerParameters>(Request.RequestScannerParameters)
            .Select(m => m.Parameters)
            .ToShareSource();

        /// <summary>
        /// Creates an observable which, upon subscription, emits scanner results, then completes.
        /// </summary>
        public IObservable<IList<ScannerDataItem>> ScannerObservable(ScannerSubscription subscription, IList<Tag>? subscriptionOptions = null, IList<Tag>? filterOptions = null)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            return Response
                .ToObservable<ScannerData>(
                    Request.NextId,
                    requestId => Request.RequestScannerSubscription(requestId, subscription, subscriptionOptions, filterOptions),
                    Request.CancelScannerSubscription)
                .Select(m => m.Items)
                .ToShareSource();
        }
    }
}
