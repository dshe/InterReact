using System.Reactive.Joins;
using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Gets the requested sequence of historical data bars.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
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
            .IgnoreAlertMessage(162)
            .AlertMessageToError()
            .Cast<HistoricalData>()
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

    /// <summary>
    /// Observable which, upon subscription, emits the requested sequence of historical data bars, plus updates.
    /// Updates continue until unsubscription.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public IObservable<HistoricalDataBar> CreateHistoricalDataContinuousObservable(
        Contract contract,
        string duration = HistoricalDataDuration.None,
        string barSize = HistoricalDataBarSize.FiveSeconds,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = false,
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
            .IgnoreAlertMessage(162)
            .AlertMessageToError()
            .SelectMany(m =>
                m switch
                {
                    HistoricalData hd => hd.Bars,
                    HistoricalDataBar hdb => new HistoricalDataBar[] { hdb },
                    _ => throw new InvalidOperationException("Unexpected type!")
                })
            .ShareSource();
    }
}
