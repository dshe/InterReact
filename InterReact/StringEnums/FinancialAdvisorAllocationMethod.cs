using StringEnums;

namespace InterReact;

public sealed class FinancialAdvisorAllocationMethod : StringEnum<FinancialAdvisorAllocationMethod>
{
    public static FinancialAdvisorAllocationMethod None { get; } = Create("");
    public static FinancialAdvisorAllocationMethod PercentChange { get; } = Create("PctChange");
    public static FinancialAdvisorAllocationMethod AvailableEquity { get; } = Create("AvailableEquity");
    public static FinancialAdvisorAllocationMethod NetLiquidity { get; } = Create("NetLiq");
    public static FinancialAdvisorAllocationMethod EqualQuantity { get; } = Create("EqualQuantity");
}
