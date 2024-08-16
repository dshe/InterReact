namespace InterReact;

public enum FundAssetType
{
    None,
    Others,
    MoneyMarket,
    FixedIncome,
    MultiAsset,
    Equity,
    Sector,
    Guaranteed,
    Alternative
}

// ???
public static class CFundAssetType
{
    public static readonly string[] values = ["None", "000", "001", "002", "003", "004", "005", "006", "007"];
    //public static readonly string[] names = { "None", "Others", "Money Market", "Fixed Income", "Multi-asset", "Equity", "Sector", "Guaranteed", "Alternative" };
    //public static string GetFundAssetTypeName(this FundAssetType e) => names[(int)e];
    public static FundAssetType GetFundAssetType(string? value) => (FundAssetType)Array.IndexOf(values, value ?? "None");
}
