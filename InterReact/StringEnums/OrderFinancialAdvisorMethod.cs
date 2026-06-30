namespace InterReact;

public sealed record OrderFinancialAdvisorMethod(string StringCode) : IHasStringCode
{
    public static readonly OrderFinancialAdvisorMethod None = new("");
    public static readonly OrderFinancialAdvisorMethod PercentChange = new("PctChange");
    public static readonly OrderFinancialAdvisorMethod AvailableEquity = new("AvailableEquity");
    public static readonly OrderFinancialAdvisorMethod NetLiquidity = new("NetLiq");
    public static readonly OrderFinancialAdvisorMethod EqualQuantity = new("EqualQuantity");
}
