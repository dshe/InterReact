namespace InterReact;

public static class ContractSecurityIdType
{
    public static readonly string Undefined = "";

    /// <summary>
    /// Example: Apple: US0378331005
    /// International.
    /// Splits usually involve a new ISIN.
    /// </summary>
    public static readonly string Isin = "ISIN";

    /// <summary>
    /// Example: Apple: 037833100
    /// North America.
    /// </summary>
    public static readonly string Cusip = "CUSIP";

    /// <summary>
    /// Consists of 6-AN + check digit. Example: BAE: 0263494
    /// UK.
    /// </summary>
    public static readonly string Sedol = "SEDOL";

    /// <summary>
    /// Consists of exchange-independent RIC Root and a suffix identifying the exchange. Example: AAPL.O for Apple on NASDAQ.
    /// </summary>
    public static readonly string Ric = "RIC";
}
