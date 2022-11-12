using StringEnums;

namespace InterReact;

public sealed class SecurityType : StringEnum<SecurityType>
{
    public static SecurityType Undefined { get; } = Create("");

    public static SecurityType Stock { get; } = Create("STK");
    public static SecurityType Option { get; } = Create("OPT");
    public static SecurityType Future { get; } = Create("FUT");
    public static SecurityType Index { get; } = Create("IND");
    public static SecurityType FutureOption { get; } = Create("FOP");
    public static SecurityType Cash { get; } = Create("CASH");

    /// <summary>
    /// For Combination Orders - must use combo leg details.
    /// </summary>
    public static SecurityType Bag { get; } = Create("BAG");
    public static SecurityType Warrant { get; } = Create("WAR");
    public static SecurityType Bond { get; } = Create("BOND");
    public static SecurityType Commodity { get; } = Create("CMDTY");
    public static SecurityType News { get; } = Create("NEWS");
    public static SecurityType Fund { get; } = Create("FUND");

    public static SecurityType Bill { get; } = Create("BILL");
    public static SecurityType ContractForDifference { get; } = Create("CFD");
    public static SecurityType IndexOption { get; } = Create("IOPT");
    public static SecurityType Forward { get; } = Create("FWD");
    public static SecurityType Fixed { get; } = Create("FIXED");
    public static SecurityType Slb { get; } = Create("SLB");
    public static SecurityType Bsk { get; } = Create("BSK");
    public static SecurityType Icu { get; } = Create("ICU");
    public static SecurityType Ics { get; } = Create("ICS");
}
