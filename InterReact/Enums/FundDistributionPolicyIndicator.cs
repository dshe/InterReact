namespace InterReact;

public enum FundDistributionPolicyIndicator
{
    None,
    AccumulationFund,
    IncomeFund
}

// ???
public static class CFundDistributionPolicyIndicator
{
    public static readonly string[] values = { "None", "N", "Y" };
    //public static readonly string[] names = { "None", "Accumulation Fund", "Income Fund" };
    //public static string GetFundDistributionPolicyIndicatorName(this FundDistributionPolicyIndicator e) => names[(int)e];
    public static FundDistributionPolicyIndicator GetFundDistributionPolicyIndicator(string value) => (FundDistributionPolicyIndicator)Array.IndexOf(values, value ?? "None");
}
