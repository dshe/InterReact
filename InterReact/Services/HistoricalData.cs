using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Observable which, upon subscription, emits the requested sequence of historical data bars, plus updates.
    /// Updates continue until unsubscription.
    /// </summary>
    public IObservable<IHasRequestId> CreateHistoricalDataObservable(
        Contract contract,
        string duration = HistoricalDataDuration.None,
        string barSize = HistoricalDataBarSize.FiveSeconds,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        int dateFormat = 1,
        params Tag[] options)
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestHistoricalData(
                    id,
                    contract,
                    "",
                    duration,
                    barSize,
                    whatToShow,
                    regularTradingHoursOnly,
                    dateFormat,
                    true,
                    options),
                Request.CancelHistoricalData)
            .SelectMany<IHasRequestId, IHasRequestId>(m =>
            {
                if (m is HistoricalData hd)
                    return hd.Bars;
                return [m];
            })
            .ShareSource();
    }

    /// <summary>
    /// Gets the requested sequence of historical data bars.
    /// Error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public async Task<HistoricalData> GetHistoricalDataAsync(
        Contract contract,
        string endDate = "",
        string duration = HistoricalDataDuration.OneMonth,
        string barSize = HistoricalDataBarSize.OneHour,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        int dateFormat = 1,  // 1: yyyyMMdd HH:mm:ss, 2: time format in seconds
        CancellationToken ct = default,
        params Tag[] options)
    {
        int id = Request.GetNextId();

        Task<HistoricalData> task = Response
            .WithRequestId(id)
            .CastTo<HistoricalData>()
            .FirstAsync()
            .ToTask(ct);

        Request.RequestHistoricalData(
                            id,
                            contract,
                            endDate,
                            duration,
                            barSize,
                            whatToShow,
                            regularTradingHoursOnly,
                            dateFormat,
                            false,
                            options);

        return await task.ConfigureAwait(false);
    }
}
