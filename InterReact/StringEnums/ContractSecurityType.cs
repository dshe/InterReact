namespace InterReact;

public sealed record ContractSecurityType(string Code) : IHasCode
{
    public static readonly ContractSecurityType Undefined = new("");

    public static readonly ContractSecurityType Stock = new("STK");
    public static readonly ContractSecurityType Option = new("OPT");
    public static readonly ContractSecurityType Future = new("FUT");
    public static readonly ContractSecurityType Index = new("IND");
    public static readonly ContractSecurityType FutureOption = new("FOP");
    public static readonly ContractSecurityType Cash = new("CASH");

    /// <summary>
    /// For Combination Orders - must use combo leg details.
    /// </summary>
    public static readonly ContractSecurityType Bag = new("BAG");
    public static readonly ContractSecurityType Warrant = new("WAR");
    public static readonly ContractSecurityType Bond = new("BOND");
    public static readonly ContractSecurityType Commodity = new("CMDTY");
    public static readonly ContractSecurityType News = new("NEWS");
    public static readonly ContractSecurityType Fund = new("FUND");

    public static readonly ContractSecurityType Bill = new("BILL");
    public static readonly ContractSecurityType ContractForDifference = new("CFD");
    public static readonly ContractSecurityType IndexOption = new("IOPT");
    public static readonly ContractSecurityType Forward = new("FWD");
    public static readonly ContractSecurityType Fixed = new("FIXED");
    public static readonly ContractSecurityType Slb = new("SLB");
    public static readonly ContractSecurityType Bsk = new("BSK");
    public static readonly ContractSecurityType Icu = new("ICU");
    public static readonly ContractSecurityType Ics = new("ICS");
}
