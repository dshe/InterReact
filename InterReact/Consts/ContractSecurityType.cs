namespace InterReact;

public static class ContractSecurityType
{
    public const string Undefined = "";

    public const string Stock = "STK";
    public const string Option = "OPT";
    public const string Future = "FUT";
    public const string Index = "IND";
    public const string FutureOption = "FOP";
    public const string Cash = "CASH";

    /// <summary>
    /// For Combination Orders - must use combo leg details.
    /// </summary>
    public const string Bag = "BAG";
    public const string Warrant = "WAR";
    public const string Bond = "BOND";
    public const string Commodity = "CMDTY";
    public const string News = "NEWS";
    public const string Fund = "FUND";

    public const string Bill = "BILL";
    public const string ContractForDifference = "CFD";
    public const string IndexOption = "IOPT";
    public const string Forward = "FWD";
    public const string Fixed = "FIXED";
    public const string Slb = "SLB";
    public const string Bsk = "BSK";
    public const string Icu = "ICU";
    public const string Ics = "ICS";
}
