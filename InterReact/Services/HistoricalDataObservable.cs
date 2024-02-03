namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Observable which, upon subscription, emits the requested sequence of historical data bars.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public IObservable<HistoricalDataBar> CreateHistoricalDataObservable(
        Contract contract,
        string endDate = "",
        string duration = HistoricalDataDuration.OneMonth,
        string barSize = HistoricalDataBarSize.OneHour,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        int dateFormat = 1,  // 1: yyyyMMdd HH:mm:ss, 2: time format in seconds
        params Tag[] options)
    {
        return Response
            .ToObservableSingleWithId(
                Request.GetNextId,
                id => Request.RequestHistoricalData(
                    id,
                    contract,
                    endDate,
                    duration,
                    barSize,
                    whatToShow,
                    regularTradingHoursOnly,
                    dateFormat,
                    false,
                    options),
                Request.CancelHistoricalData)
            .IgnoreAlertMessage(162)
            .AlertMessageToError()
            .Cast<HistoricalData>()
            .SelectMany(hd => hd.Bars)
            .ShareSource();
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
