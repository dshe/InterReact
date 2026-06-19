namespace InterReact;
//https://ibkrcampus.com/campus/ibkr-api-page/twsapi-doc/#api-error-codes

public sealed record AlertDefinition
{
    public int Code { get; }
    public string Message { get; }
    private AlertDefinition(int code, string message)
    {
        Code = code;
        Message = message;
        _alerts.Add(code, this);
    }

    private static readonly Dictionary<int, AlertDefinition> _alerts = [];
    public static AlertDefinition? GetAlert(int code) => _alerts.TryGetValue(code, out AlertDefinition? alert) ? alert : null;
    // No market data permissions
    // Time length exceed max
    // Invalid step
    // No data available weekends(?)
    // API historical data query cancelled
    //public static readonly ErrorResponse HistoricalDataServiceError = new (162, "Historical market data Service error message.");
    public static readonly AlertDefinition MarketDataNotSubscribed = new(10167, "Requested market data is not subscribed. Displaying delayed market data.");
    public static readonly AlertDefinition MarketDataNotSupported  = new(10168, "Requested market data is not supported. Displaying delayed market data.");
}
