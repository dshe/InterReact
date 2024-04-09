namespace InterReact;

public sealed class Contract // input + output OK
{
    /// <summary>
    /// The unique contract identifier.
    /// </summary>
    public int ContractId { get; set; }

    /// <summary>
    /// The underlying's asset symbol
    /// </summary>
    public string Symbol { get; set; } = "";

    public string SecurityType { get; set; } = ContractSecurityType.Undefined;

    /// <summary>
    /// The contract's last trading day or contract month (for Options and Futures). 
    /// Strings with format YYYYMM will be interpreted as the Contract month,
    /// whereas YYYYMMDD will be interpreted as Last Trading Day
    /// </summary>
    public string LastTradeDateOrContractMonth { get; set; } = "";  // Expiry: YYYYMM or YYYYMMDD

    public double Strike { get; set; }
    public string Right { get; set; } = ContractOptionRight.Undefined;
    public string Multiplier { get; set; } = ""; // options and futures

    /// <summary>
    /// The exchange where the contract is traded or the the order destination. For example: ARCA, SMART.
    /// </summary>
    public string Exchange { get; set; } = "";

    /// <summary>
    /// The underlying's currency
    /// </summary>
    public string Currency { get; set; } = "";

    /// <summary>
    /// The contract's symbol on the primary exchange.
    /// For options, this will be the OCC symbol.
    /// </summary>
    public string LocalSymbol { get; set; } = "";

    /// <summary>
    /// The non-aggregate (not SMART) primary exchange.
    /// For exchanges which contain a period in name, will only be part of exchange name prior to period, i.e. ENEXT for ENEXT.BE
    /// </summary>
    public string PrimaryExchange { get; set; } = "";

    /// <summary>
    /// The trading class name for this contract.
    /// Available in TWS contract description window as well. For example, GBL Dec '13 future's trading class is "FGBL"
    /// </summary>
    public string TradingClass { get; set; } = "";

    /// <summary>
    /// If set to true, contract details requests and historical data queries can be performed pertaining to expired contracts.
    /// Historical data queries on expired contracts are limited to the last year of the contracts life,
    /// and are initially only supported for expired futures contracts.
    /// If set to true, contract details requests and historical data queries can be performed pertaining to expired futures contracts.
    /// Expired options or other instrument types are not available.
    /// </summary>
    public bool IncludeExpired { get; set; } // input only
    public string SecurityIdType { get; set; } = ContractSecurityIdType.Undefined; // input only
    public string SecurityId { get; set; } = ""; // input only

    public string Description { get; set; } = "";
    public string IssuerId { get; set; } = "";

    public string ComboLegsDescription { get; set; } = "";

    /// <summary>
    /// The legs of a combined contract definition.
    /// </summary>
    public IList<ContractComboLeg> ComboLegs { get; } = new List<ContractComboLeg>(); // input + output

    /// <summary>
    /// Delta and underlying price for Delta-Neutral combo orders.
    /// </summary>
    public DeltaNeutralContract? DeltaNeutralContract { get; set; }

    public Contract() { }
    internal Contract(ResponseReader r, bool includeExchange = true, bool includePrimaryExchange = true)
    {
        ContractId = r.ReadInt();
        Symbol = r.ReadString();
        SecurityType = r.ReadString();
        LastTradeDateOrContractMonth = r.ReadString();
        Strike = r.ReadDouble();
        Right = r.ReadString();
        Multiplier = r.ReadString();
        if (includeExchange)
            Exchange = r.ReadString();
        if (includePrimaryExchange)
            PrimaryExchange = r.ReadString();
        Currency = r.ReadString();
        LocalSymbol = r.ReadString();
        TradingClass = r.ReadString();
    }

    internal RequestMessage Write(RequestMessage m, bool includePrimaryExchange, bool includeExpired)
    {
        m.Write(
            ContractId,
            Symbol,
            SecurityType,
            LastTradeDateOrContractMonth,
            Strike,
            Right,
            Multiplier,
            Exchange);

        if (includePrimaryExchange)
            m.Write(PrimaryExchange);

        m.Write(
            Currency,
            LocalSymbol,
            TradingClass);

        if (includeExpired)
            m.Write(IncludeExpired);

        return m;
    }
}
