namespace InterReact;

public sealed record OrderAction(string StringCode) : IHasStringCode
{
    public static readonly OrderAction Undefined = new("");

    public static readonly OrderAction Buy = new("BUY");
    public static readonly OrderAction Sell = new("SELL");

    /// <summary>
    /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
    /// </summary>
    public static readonly OrderAction SellShort = new("SSHORT");
}
