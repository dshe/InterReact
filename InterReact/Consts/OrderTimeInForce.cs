namespace InterReact;

public static class OrderTimeInForce
{
    /// <summary>
    /// Order valid for the current day. This is the default;
    /// </summary>
    public const string Day = "DAY";

    /// <summary>
    /// Good Till Cancelled.
    /// </summary>
    public const string GoodUntilCancelled = "GTC";

    /// <summary>
    /// You can set the time in force for MARKET or LIMIT orders as IOC.
    /// This dictates that any portion of the order not executed immediately after it becomes available on the market will be cancelled.
    /// </summary>
    public const string ImmediateOrCancel = "IOC";

    public const string GoodUntilDate = "GTD";

    public const string LimitOrMarketOnOpen = "OPG";

    /// <summary>
    /// Setting FOK as the time in force dictates that the entire order must execute immediately or be canceled.
    /// </summary>
    public const string FillOrKill = "FOK";

    public const string DayUntilCancelled = "DTC";

    public const string Auction = "AUC";
}
