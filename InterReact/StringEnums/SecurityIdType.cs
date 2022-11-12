using StringEnums;

namespace InterReact;

public sealed class SecurityIdType : StringEnum<SecurityIdType>
{
    public static SecurityIdType Undefined { get; } = Create("");

    /// <summary>
    /// Example: Apple: US0378331005
    /// International.
    /// Splits usually involve a new ISIN.
    /// </summary>
    public static SecurityIdType Isin { get; } = Create("ISIN");

    /// <summary>
    /// Example: Apple: 037833100
    /// North America.
    /// </summary>
    public static SecurityIdType Cusip { get; } = Create("CUSIP");

    /// <summary>
    /// Consists of 6-AN + check digit. Example: BAE: 0263494
    /// UK.
    /// </summary>
    public static SecurityIdType Sedol { get; } = Create("SEDOL");

    /// <summary>
    /// Consists of exchange-independent RIC Root and a suffix identifying the exchange. Example: AAPL.O for Apple on NASDAQ.
    /// </summary>
    public static SecurityIdType Ric { get; } = Create("RIC");
}
