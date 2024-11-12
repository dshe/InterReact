namespace InterReact;

public static class OrderTimeInForce
{
    /// <summary>
    /// Order valid for the current day. This is the default;
    /// </summary>
    public static readonly string Day = "DAY";

    /// <summary>
    /// Good Till Cancelled.
    /// </summary>
    public static readonly string GoodUntilCancelled = "GTC";

    /// <summary>
    /// You can set the time in force for MARKET or LIMIT orders as IOC.
    /// This dictates that any portion of the order not executed immediately after it becomes available on the market will be cancelled.
    /// </summary>
    public static readonly string ImmediateOrCancel = "IOC";

    public static readonly string GoodUntilDate = "GTD";

    public static readonly string LimitOrMarketOnOpen = "OPG";

    /// <summary>
    /// Setting FOK as the time in force dictates that the entire order must execute immediately or be canceled.
    /// </summary>
    public static readonly string FillOrKill = "FOK";

    public static readonly string DayUntilCancelled = "DTC";

    public static readonly string Auction = "AUC";
}
