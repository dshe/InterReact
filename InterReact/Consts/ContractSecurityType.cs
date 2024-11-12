namespace InterReact;

public static class ContractSecurityType
{
    public static readonly string Undefined = "";

    public static readonly string Stock = "STK";
    public static readonly string Option = "OPT";
    public static readonly string Future = "FUT";
    public static readonly string Index = "IND";
    public static readonly string FutureOption = "FOP";
    public static readonly string Cash = "CASH";

    /// <summary>
    /// For Combination Orders - must use combo leg details.
    /// </summary>
    public static readonly string Bag = "BAG";
    public static readonly string Warrant = "WAR";
    public static readonly string Bond = "BOND";
    public static readonly string Commodity = "CMDTY";
    public static readonly string News = "NEWS";
    public static readonly string Fund = "FUND";

    public static readonly string Bill = "BILL";
    public static readonly string ContractForDifference = "CFD";
    public static readonly string IndexOption = "IOPT";
    public static readonly string Forward = "FWD";
    public static readonly string Fixed = "FIXED";
    public static readonly string Slb = "SLB";
    public static readonly string Bsk = "BSK";
    public static readonly string Icu = "ICU";
    public static readonly string Ics = "ICS";
}
