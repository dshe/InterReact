using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NodaTime;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which emits historical data, then completes.
        /// This method accepts arguments which will not necessarily be accepted by Tws/Gateway.
        /// Note that IB imposes limitations on the timing of historical data requests.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<HistoricalData, Alert>> CreateHistoricalDataObservable(
            Contract contract,
            HistoricalBarSize? barSize = null,
            HistoricalDuration? duration = null,
            Instant end = default,
            HistoricalDataType? dataType = null,
            bool regularTradingHoursOnly = true,
            IEnumerable<Tag>? options = null) =>
                Response
                    .ToObservableSingleWithId(
                        Request.GetNextId,
                        id => Request.RequestHistoricalData(
                            id,
                            contract,
                            end,
                            duration,
                            barSize,
                            dataType,
                            regularTradingHoursOnly,
                            options),
                        Request.CancelHistoricalData)
                    .Select(x => new Union<HistoricalData, Alert>(x));
    }
}
