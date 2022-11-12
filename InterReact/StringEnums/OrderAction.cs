using StringEnums;

namespace InterReact;

public sealed class OrderAction : StringEnum<OrderAction>
{
    public static OrderAction Undefined { get; } = Create("");

    public static OrderAction Buy { get; } = Create("BUY");
    public static OrderAction Sell { get; } = Create("SELL");

    /// <summary>
    /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
    /// </summary>
    public static OrderAction SellShort { get; } = Create("SSHORT");

    /// <summary>
    /// Allows orders to be marked as exempt from SEC Rule 201.
    /// </summary>
    //public static TradeAction SellShortExemptFromSecRule201 { get; } = Create("SSHORTX");
}
