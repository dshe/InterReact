namespace InterReact;

public sealed record OrderTimeInForce(string Code) : IHasCode
{
    /// <summary>
    /// Order valid for the current day. This is the default;
    /// </summary>
    public static readonly OrderTimeInForce Day = new("DAY");

    /// <summary>
    /// Good Till Cancelled.
    /// </summary>
    public static readonly OrderTimeInForce GoodUntilCancelled = new("GTC");

    /// <summary>
    /// You can set the time in force for MARKET or LIMIT orders as IOC.
    /// This dictates that any portion of the order not executed immediately after it becomes available on the market will be cancelled.
    /// </summary>
    public static readonly OrderTimeInForce ImmediateOrCancel = new("IOC");

    public static readonly OrderTimeInForce GoodUntilDate = new("GTD");

    public static readonly OrderTimeInForce LimitOrMarketOnOpen = new("OPG");

    /// <summary>
    /// Setting FOK as the time in force dictates that the entire order must execute immediately or be canceled.
    /// </summary>
    public static readonly OrderTimeInForce FillOrKill = new("FOK");

    public static readonly OrderTimeInForce DayUntilCancelled = new("DTC");

    public static readonly OrderTimeInForce Auction = new("AUC");
}
