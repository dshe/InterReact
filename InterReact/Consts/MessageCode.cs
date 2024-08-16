namespace InterReact;

//https://ibkrcampus.com/campus/ibkr-api-page/twsapi-doc/#api-error-codes

public class ErrorResponse
{
    public ErrorResponse(int Code, string Message) => (this.Code, this.Message) = (Code, Message);
    public int Code { get; }
    public string Message { get; }

    // No market data permissions
    // Time length exceed max
    // Invalid step
    // No data available weekends(?)
    // API historical data query cancelled
    public static readonly ErrorResponse HistoricalDataServiceError = new (162, "Historical market data Service error message.");

    public static readonly ErrorResponse MarketDataNotSubscribed = new (10167, "Requested market data is not subscribed. Displaying delayed market data.");

    //public static readonly ErrorResponse MarketDataNotSupported = new (10168, "Requested market data is not supported. Displaying delayed market data.");
}
