﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which continually emits 5 second bars.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<Union<RealtimeBar, Alert>> CreateRealtimeBarObservable(Contract contract,
            RealtimeBarType? whatToShow = null, bool regularTradingHours = true, IList<Tag>? options = null)
        {
            whatToShow ??= RealtimeBarType.Trades;

            return Response.ToObservableWithIdContinuous(
                    Request.GetNextId,
                    id => Request.RequestRealTimeBars(id, contract, whatToShow, regularTradingHours, options),
                    id => Request.CancelRealTimeBars(id))
                .Select(x => new Union<RealtimeBar, Alert>(x))
                .Publish().RefCount();
        }
    }

}
