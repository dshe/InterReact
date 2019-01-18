using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class FinancialAdvisorAllocationMethod : StringEnum<FinancialAdvisorAllocationMethod>
    {
        public static readonly FinancialAdvisorAllocationMethod None = Create("");
        public static readonly FinancialAdvisorAllocationMethod PercentChange = Create("PctChange");
        public static readonly FinancialAdvisorAllocationMethod AvailableEquity = Create("AvailableEquity");
        public static readonly FinancialAdvisorAllocationMethod NetLiquidity = Create("NetLiq");
        public static readonly FinancialAdvisorAllocationMethod EqualQuantity = Create("EqualQuantity");
    }
}
