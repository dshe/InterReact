using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NodaTime;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits historical data.
        /// This method accepts arguments which will not necessarily be accepted by Tws/Gateway.
        /// Note that IB imposes limitations on the timing of historical data requests.
        /// </summary>
        public IObservable<IHistoricalData> CreateHistoricalDataObservable(
            Contract contract,
            HistoricalBarSize? barSize = null,
            HistoricalDuration? duration = null,
            Instant end = default,
            HistoricalDataType? dataType = null,
            bool regularTradingHoursOnly = true,
            IList<Tag>? options = null) =>
                Response
                    .ToObservableWithIdSingle<IHistoricalData>(
                        Request.GetNextId,
                        requestId => Request.RequestHistoricalData(
                            requestId,
                            contract,
                            end,
                            duration,
                            barSize,
                            dataType,
                            regularTradingHoursOnly,
                            options),
                        Request.CancelHistoricalData)
                    .ToShareSource();
    }

    public static partial class Extensions
    {
        public static IObservable<HistoricalData> OfTypeHistoricalData(this IObservable<IHistoricalData> source) =>
            source.OfType<HistoricalData>();
    }

}
