namespace InterReact;

public static class OrderAction
{
    public const string Undefined = "";

    public const string Buy = "BUY";
    public const string Sell = "SELL";

    /// <summary>
    /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
    /// </summary>
    public const string SellShort = "SSHORT";

    /// <summary>
    /// Allows orders to be marked as exempt from SEC Rule 201.
    /// </summary>
    //public static TradeAction SellShortExemptFromSecRule201 { get; } = Create("SSHORTX");
}
