using StringEnums;

namespace InterReact;

public sealed class TimeInForce : StringEnum<TimeInForce>
{
    /// <summary>
    /// Order valid for the current day. This is the default;
    /// </summary>
    public static TimeInForce Day { get; } = Create("DAY");

    /// <summary>
    /// Good Till Cancelled.
    /// </summary>
    public static TimeInForce GoodUntilCancelled { get; } = Create("GTC");

    /// <summary>
    /// You can set the time in force for MARKET or LIMIT orders as IOC.
    /// This dictates that any portion of the order not executed immediately after it becomes available on the market will be cancelled.
    /// </summary>
    public static TimeInForce ImmediateOrCancel { get; } = Create("IOC");

    public static TimeInForce GoodUntilDate { get; } = Create("GTD");

    public static TimeInForce LimitOrMarketOnOpen { get; } = Create("OPG");

    /// <summary>
    /// Setting FOK as the time in force dictates that the entire order must execute immediately or be canceled.
    /// </summary>
    public static TimeInForce FillOrKill { get; } = Create("FOK");

    public static TimeInForce DayUntilCancelled { get; } = Create("DTC");
}
