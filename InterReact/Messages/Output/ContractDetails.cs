using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InterReact;

internal enum ContractDetailsType { GeneralContractType, BondContractType, ScannerContractType }

public sealed class ContractDetails : IHasRequestId // output
{
    /// <summary>
    /// The Id that was used to make the request to receive this ContractDetails.
    /// </summary>
    public int RequestId { get; }

    public Contract Contract { get; } = new();

    public string MarketName { get; } = "";
    public double MinimumTick { get; }

    /// <summary>
    /// Allows execution and strike prices to be reported consistently with market data, historical data and the order price,
    /// i.e. Z on LIFFE is reported in index points and not GBP.
    /// </summary>
    public int PriceMagnifier { get; }

    /// <summary>
    /// The list of valid order types for this contract.
    /// </summary>
    public string OrderTypes { get; } = "";

    /// <summary>
    /// The list of exchanges this contract is traded on.
    /// </summary>
    public string ValidExchanges { get; } = "";

    public int UnderlyingContractId { get; }

    public string LongName { get; } = "";

    /// <summary>
    /// The contract month. Typically the contract month of the underlying for a futures contract.
    /// </summary>
    public string ContractMonth { get; } = "";

    /// <summary>
    /// The industry classification of the underlying/product. 
    /// </summary>
    public string Industry { get; } = "";

    /// <summary>
    /// The industry category of the underlying.
    /// </summary>
    public string Category { get; } = "";

    public string Subcategory { get; } = "";

    /// <summary>
    /// The ID of the time zone for the trading hours of the product. For example, EST.
    /// </summary>
    public string TimeZoneId { get; private set; } = "";

    /// <summary>
    /// The total trading hours of the product. For example, 20090507:0700-1830,1830-2330;20090508:CLOSED.
    /// </summary>
    public string TradingHours { get; } = "";

    /// <summary>
    /// The liquid trading hours of the product. For example, 20090507:0930-1600;20090508:CLOSED.
    /// </summary>
    public string LiquidHours { get; } = "";

    /// <summary>
    /// For products in Australia which trade in non-currency units.
    /// This string attribute contains the Economic Value Rule name and the respective optional argument.
    /// The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. 
    /// When the optional argument is not present, the first value will be followed by a colon.
    /// </summary>
    public string EconomicValueRule { get; } = "";

    /// <summary>
    /// For products in Australia which trade in non-currency units.
    /// This double attribute tells you approximately how much the market value of a contract would change if the price were to change by 1.
    /// It cannot be used to get market value by multiplying the price by the approximate multiplier.
    /// </summary>
    public double EconomicValueMultiplier { get; }

    public int MdSizeMultiplier { get; }

    public int AggGroup { get; }

    /// <summary>
    /// CUSIP, ISIN etc.
    /// </summary>
    public IList<Tag> SecurityIds { get; } = new List<Tag>(); // output

    public string UnderSymbol { get; } = "";
    public string UnderSecType { get; } = "";
    public string MarketRuleIds { get; } = "";
    public string RealExpirationDate { get; } = "";
    public string LastTradeTime { get; private set; } = "";
    public string StockType { get; } = "";

    // BOND Values

    /// <summary>
    /// The nine-character bond CUSIP or the 12-character SEDOL.
    /// </summary>
    public string Cusip { get; } = "";
    public string CreditRatings { get; } = "";
    public string DescriptionAppend { get; } = "";
    public string BondType { get; } = "";
    public string CouponType { get; } = "";
    public bool Callable { get; }
    public bool Putable { get; }
    public double Coupon { get; }
    public bool Convertible { get; }
    public string Maturity { get; private set; } = "";
    public string IssueDate { get; } = "";
    public string NextOptionDate { get; } = "";

    /// <summary>
    /// If bond has embedded options.
    /// </summary>
    public string NextOptionType { get; } = "";

    /// <summary>
    /// If bond has embedded options.
    /// </summary>
    public bool NextOptionPartial { get; }

    public string Notes { get; } = "";

    internal ContractDetails() { }

    internal ContractDetails(ResponseReader r, ContractDetailsType type)
    {
        switch (type)
        {
            case ContractDetailsType.GeneralContractType:
                r.RequireVersion(8);
                RequestId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<SecurityType>();
                ReadLastTradeDate(r.ReadString(), false);
                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadStringEnum<OptionRightType>();
                Contract.Exchange = r.ReadString();
                Contract.Currency = r.ReadString();
                Contract.LocalSymbol = r.ReadString();
                MarketName = r.ReadString();
                Contract.TradingClass = r.ReadString();
                Contract.ContractId = r.ReadInt();
                MinimumTick = r.ReadDouble();
                if (r.Connector.SupportsServerVersion(ServerVersion.MD_SIZE_MULTIPLIER))
                    MdSizeMultiplier = r.ReadInt();
                Contract.Multiplier = r.ReadString();
                OrderTypes = r.ReadString();
                ValidExchanges = r.ReadString();
                PriceMagnifier = r.ReadInt();
                UnderlyingContractId = r.ReadInt();
                LongName = r.ReadString();
                Contract.PrimaryExchange = r.ReadString();
                ContractMonth = r.ReadString();
                Industry = r.ReadString();
                Category = r.ReadString();
                Subcategory = r.ReadString();
                TimeZoneId = r.ReadString();
                TradingHours = r.ReadString();
                LiquidHours = r.ReadString();
                EconomicValueRule = r.ReadString();
                EconomicValueMultiplier = r.ReadDouble();
                r.AddTagsToList(SecurityIds);
                if (r.Connector.SupportsServerVersion(ServerVersion.AGG_GROUP))
                    AggGroup = r.ReadInt();
                if (r.Connector.SupportsServerVersion(ServerVersion.UNDERLYING_INFO))
                {
                    UnderSymbol = r.ReadString();
                    UnderSecType = r.ReadString();
                }
                if (r.Connector.SupportsServerVersion(ServerVersion.MARKET_RULES))
                    MarketRuleIds = r.ReadString();
                if (r.Connector.SupportsServerVersion(ServerVersion.REAL_EXPIRATION_DATE))
                    RealExpirationDate = r.ReadString();
                if (r.Connector.SupportsServerVersion(ServerVersion.STOCK_TYPE))
                    StockType = r.ReadString();
                break;

            case ContractDetailsType.BondContractType:
                r.RequireVersion(6);
                RequestId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<SecurityType>();
                Cusip = r.ReadString();
                Coupon = r.ReadDouble();
                ReadLastTradeDate(r.ReadString(), true);

                IssueDate = r.ReadString();
                CreditRatings = r.ReadString();
                BondType = r.ReadString();
                CouponType = r.ReadString();
                Convertible = r.ReadBool();
                Callable = r.ReadBool();
                Putable = r.ReadBool();
                DescriptionAppend = r.ReadString();

                Contract.Exchange = r.ReadString();
                Contract.Currency = r.ReadString();
                MarketName = r.ReadString();
                Contract.TradingClass = r.ReadString();
                Contract.ContractId = r.ReadInt();
                MinimumTick = r.ReadDouble();
                if (r.Connector.SupportsServerVersion(ServerVersion.MD_SIZE_MULTIPLIER))
                    MdSizeMultiplier = r.ReadInt();
                OrderTypes = r.ReadString();
                ValidExchanges = r.ReadString();
                NextOptionDate = r.ReadString();
                NextOptionType = r.ReadString();
                NextOptionPartial = r.ReadBool();
                Notes = r.ReadString();
                LongName = r.ReadString();
                EconomicValueRule = r.ReadString();
                EconomicValueMultiplier = r.ReadDouble();
                r.AddTagsToList(SecurityIds);
                if (r.Connector.SupportsServerVersion(ServerVersion.AGG_GROUP))
                    AggGroup = r.ReadInt();
                if (r.Connector.SupportsServerVersion(ServerVersion.MARKET_RULES))
                    MarketRuleIds = r.ReadString();
                break;

            case ContractDetailsType.ScannerContractType:
                Contract.ContractId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<SecurityType>();
                Contract.LastTradeDateOrContractMonth = r.ReadString();
                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadStringEnum<OptionRightType>();
                Contract.Exchange = r.ReadString();
                Contract.Currency = r.ReadString();
                Contract.LocalSymbol = r.ReadString();
                MarketName = r.ReadString();
                Contract.TradingClass = r.ReadString();
                break;
        }
    }

    private void ReadLastTradeDate(string lastTradeDateOrContractMonth, bool isBond)
    {
        if (lastTradeDateOrContractMonth.Length == 0)
            return;
        string[] split = Regex.Split(lastTradeDateOrContractMonth, "\\s+");
        if (split.Length > 0)
        {
            if (isBond)
                Maturity = split[0];
            else
                Contract.LastTradeDateOrContractMonth = split[0];
        }
        if (split.Length > 1)
            LastTradeTime = split[1];
        if (isBond && split.Length > 2)
            TimeZoneId = split[2];
    }
}

public sealed class ContractDetailsEnd : IHasRequestId
{
    public int RequestId { get; }
    internal ContractDetailsEnd(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
    }
}
