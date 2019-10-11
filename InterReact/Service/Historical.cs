using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Extensions;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility.Rx;
using NodaTime;

#nullable enable

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits historical data.
        /// This method accepts arguments which will not necessarily be accepted by Tws/Gateway.
        /// Note that IB imposes limitations on the timing of historical data requests.
        /// </summary>
        public IObservable<HistoricalBars> HistoricalDataObservable(
            Contract contract,
            HistoricalBarSize? barSize = null,
            HistoricalDuration? duration = null,
            Instant end = default,
            HistoricalDataType? dataType = null,
            bool regularTradingHoursOnly = true,
            bool keepUpToDate = false,
            IList<Tag>? options = null)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            return Response
                .ToObservable<HistoricalBars>(
                    Request.NextId,
                    requestId => Request.RequestHistoricalData(requestId, contract, end, duration, barSize, dataType, regularTradingHoursOnly, keepUpToDate, options),
                    Request.CancelHistoricalData)
                .ToShareSource();
        }
    }
}
