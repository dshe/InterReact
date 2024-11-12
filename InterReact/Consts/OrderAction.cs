namespace InterReact;

public static class OrderAction
{
    public static readonly string Undefined = "";

    public static readonly string Buy = "BUY";
    public static readonly string Sell = "SELL";

    /// <summary>
    /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
    /// </summary>
    public static readonly string SellShort = "SSHORT";

    /// <summary>
    /// Allows orders to be marked as exempt from SEC Rule 201.
    /// </summary>
    //public static TradeAction SellShortExemptFromSecRule201 { get; } = Create("SSHORTX");
}
