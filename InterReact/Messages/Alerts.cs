namespace InterReact;

//https://ibkrcampus.com/campus/ibkr-api-page/twsapi-doc/#api-error-codes

public sealed class Alert
{
    public int Code { get; }
    public bool IsFatal { get; }
    public string Message { get; }
    Alert(int code, bool isFatal, string message)
    {
        Code = code;
        IsFatal = isFatal;
        Message = message;
        Alerts.Add(code, this);
    }

    private static Dictionary<int, Alert> Alerts = [];
    public static Alert? GetAlert(int code) => Alerts.TryGetValue(code, out Alert? alert) ? alert : null;

    // No market data permissions
    // Time length exceed max
    // Invalid step
    // No data available weekends(?)
    // API historical data query cancelled
    //public static readonly ErrorResponse HistoricalDataServiceError = new (162, "Historical market data Service error message.");
    public static readonly Alert MarketDataNotSubscribed = new (10167, false, "Requested market data is not subscribed. Displaying delayed market data.");
    public static readonly Alert MarketDataNotSupported  = new (10168, false, "Requested market data is not supported. Displaying delayed market data.");
}
