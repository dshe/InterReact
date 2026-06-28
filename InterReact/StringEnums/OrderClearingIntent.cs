namespace InterReact;

public sealed record OrderClearingIntent(string Code) : IHasCode
{
    public static readonly OrderClearingIntent Default = new("");
    public static readonly OrderClearingIntent Ib = new("IB");
    public static readonly OrderClearingIntent Away = new("Away");
    public static readonly OrderClearingIntent PostTradeAllocation = new("PTA");
}
