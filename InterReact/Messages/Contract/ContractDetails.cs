using System.IO;
using System.Text.RegularExpressions;
namespace InterReact;

[Message]
public sealed record ContractDetails : IHasRequestId
{
    /// <summary>
    /// The Id that was used to make the request to receive this ContractDetails.
    /// </summary>
    public int RequestId { get; init; }

    public Contract Contract { get; } = new();

    public string MarketName { get; init; } = "";
    public double MinimumTick { get; init; }

    /// <summary>
    /// Allows execution and strike prices to be reported consistently with market data, historical data and the order price,
    /// i.e. Z on LIFFE is reported in index points and not GBP.
    /// </summary>
    public int PriceMagnifier { get; init; }

    /// <summary>
    /// The list of valid order types for this contract.
    /// </summary>
    public string OrderTypes { get; init; } = "";

    /// <summary>
    /// The list of exchanges this contract is traded on.
    /// </summary>
    public string ValidExchanges { get; init; } = "";

    public int UnderlyingContractId { get; init; }

    public string LongName { get; init; } = "";

    /// <summary>
    /// The contract month. Typically the contract month of the underlying for a futures contract.
    /// </summary>
    public string ContractMonth { get; init; } = "";

    /// <summary>
    /// The industry classification of the underlying/product. 
    /// </summary>
    public string Industry { get; init; } = "";

    /// <summary>
    /// The industry category of the underlying.
    /// </summary>
    public string Category { get; init; } = "";

    public string Subcategory { get; init; } = "";

    /// <summary>
    /// The ID of the time zone for the trading hours of the product. For example, EST.
    /// </summary>
    public string TimeZoneId { get; set; } = "";

    /// <summary>
    /// The total trading hours of the product. For example, 20090507:0700-1830,1830-2330;20090508:CLOSED.
    /// </summary>
    public string TradingHours { get; init; } = "";

    /// <summary>
    /// The liquid trading hours of the product. For example, 20090507:0930-1600;20090508:CLOSED.
    /// </summary>
    public string LiquidHours { get; init; } = "";

    /// <summary>
    /// For products in Australia which trade in non-currency units.
    /// This string attribute contains the Economic Value Rule name and the respective optional argument.
    /// The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. 
    /// When the optional argument is not present, the first value will be followed by a colon.
    /// </summary>
    public string EconomicValueRule { get; init; } = "";

    /// <summary>
    /// For products in Australia which trade in non-currency units.
    /// This double attribute tells you approximately how much the market value of a contract would change if the price were to change by 1.
    /// It cannot be used to get market value by multiplying the price by the approximate multiplier.
    /// </summary>
    public double EconomicValueMultiplier { get; init; }

    //public int MdSizeMultiplier { get; }

    public int AggGroup { get; init; }

    /// <summary>
    /// CUSIP, ISIN etc.
    /// </summary>
    public IList<Tag> SecurityIds { get; init; } = [];

    public string UnderSymbol { get; init; } = "";
    public string UnderSecType { get; init; } = "";
    public string MarketRuleIds { get; init; } = "";
    public string RealExpirationDate { get; init; } = "";
    public string LastTradeTime { get; private set; } = "";
    public string StockType { get; init; } = "";

    // BOND Values

    /// <summary>
    /// The nine-character bond CUSIP or the 12-character SEDOL.
    /// </summary>
    public string Cusip { get; init; } = "";
    public string CreditRatings { get; init; } = "";
    public string DescriptionAppend { get; init; } = "";
    public string BondType { get; init; } = "";
    public string CouponType { get; init; } = "";
    public bool Callable { get; init; }
    public bool Putable { get; init; }
    public double Coupon { get; init; }
    public bool Convertible { get; init; }
    public string Maturity { get; private set; } = "";
    public string IssueDate { get; init; } = "";
    public string NextOptionDate { get; init; } = "";
    public string NextOptionType { get; init; } = "";
    public bool NextOptionPartial { get; init; }
    public string Notes { get; init; } = "";
    public decimal MinSize { get; init; } = decimal.MaxValue;
    public decimal SizeIncrement { get; init; } = decimal.MaxValue;
    public decimal SuggestedSizeIncrement { get; init; } = decimal.MaxValue;
    public string FundName { get; init; } = "";
    public string FundFamily { get; init; } = "";
    public string FundType { get; init; } = "";
    public string FundFrontLoad { get; init; } = "";
    public string FundBackLoad { get; init; } = "";
    public string FundBackLoadTimeInterval { get; init; } = "";
    public string FundManagementFee { get; init; } = "";
    public bool FundClosed { get; init; }
    public bool FundClosedForNewInvestors { get; init; }
    public bool FundClosedForNewMoney { get; init; }
    public string FundNotifyAmount { get; init; } = "";
    public string FundMinimumInitialPurchase { get; init; } = "";
    public string FundSubsequentMinimumPurchase { get; init; } = "";
    public string FundBlueSkyStates { get; init; } = "";
    public string FundBlueSkyTerritories { get; init; } = "";
    public FundDistributionPolicyIndicator FundDistributionPolicyIndicator { get; init; }
    public FundAssetType FundAssetType { get; init; }
    public IList<IneligibilityReason> IneligibilityReasonList { get; init; } = [];
    internal ContractDetails() { }
    internal ContractDetails(ResponseReader r, ContractDetailsType type)
    {
        switch (type)
        {
            case ContractDetailsType.GeneralContractType:
                RequestId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<ContractSecurityType>();

                ReadLastTradeDate(r.ReadString(), false);
                if (r.Options.ServerVersionCurrent >= ServerVersion.LAST_TRADE_DATE)
                    Contract.LastTradeDateOrContractMonth = r.ReadString(); // ???

                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadStringEnum<ContractOptionRight>();
                Contract.Exchange = r.ReadString();
                Contract.Currency = r.ReadString();
                Contract.LocalSymbol = r.ReadString();
                MarketName = r.ReadString();
                Contract.TradingClass = r.ReadString();
                Contract.ContractId = r.ReadInt();
                MinimumTick = r.ReadDouble();
                Contract.Multiplier = r.ReadString();
                OrderTypes = r.ReadString();
                ValidExchanges = r.ReadString();
                PriceMagnifier = r.ReadInt();
                UnderlyingContractId = r.ReadInt();
                LongName = Regex.Unescape(r.ReadString());
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
                r.AddToTags(SecurityIds);
                AggGroup = r.ReadInt();
                UnderSymbol = r.ReadString();
                UnderSecType = r.ReadString();
                MarketRuleIds = r.ReadString();
                RealExpirationDate = r.ReadString();
                StockType = r.ReadString();
                MinSize = r.ReadDecimal();
                SizeIncrement = r.ReadDecimal();
                SuggestedSizeIncrement = r.ReadDecimal();

                /*
                if (r.Options.ServerVersionCurrent >= ServerVersion.FUND_DATA_FIELDS && Contract.SecurityType == "FUND")
                {
                    FundName = r.ReadString();
                    FundFamily = r.ReadString();
                    FundType = r.ReadString();
                    FundFrontLoad = r.ReadString();
                    FundBackLoad = r.ReadString();
                    FundBackLoadTimeInterval = r.ReadString();
                    FundManagementFee = r.ReadString();
                    FundClosed = r.ReadBool();
                    FundClosedForNewInvestors = r.ReadBool();
                    FundClosedForNewMoney = r.ReadBool();
                    FundNotifyAmount = r.ReadString();
                    FundMinimumInitialPurchase = r.ReadString();
                    FundSubsequentMinimumPurchase = r.ReadString();
                    FundBlueSkyStates = r.ReadString();
                    FundBlueSkyTerritories = r.ReadString();
                    FundDistributionPolicyIndicator = CFundDistributionPolicyIndicator.GetFundDistributionPolicyIndicator(r.ReadString());
                    FundAssetType = CFundAssetType.GetFundAssetType(r.ReadString());
                }

                if (r.Options.ServerVersionCurrent >= ServerVersion.INELIGIBILITY_REASONS)
                {
                    var n = r.ReadInt();
                    if (n > 0)
                    {
                        for (var i = 0; i < n; ++i)
                        {
                            IneligibilityReasonList.Add(new IneligibilityReason
                            {
                                Id = r.ReadString(),
                                Description = r.ReadString()
                            });
                        }
                    }
                }
                */
                break;

            case ContractDetailsType.BondContractType:
                RequestId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<ContractSecurityType>();
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
                OrderTypes = r.ReadString();
                ValidExchanges = r.ReadString();
                NextOptionDate = r.ReadString();
                NextOptionType = r.ReadString();
                NextOptionPartial = r.ReadBool();
                Notes = r.ReadString();
                LongName = r.ReadString();
                EconomicValueRule = r.ReadString();
                EconomicValueMultiplier = r.ReadDouble();
                r.AddToTags(SecurityIds);
                AggGroup = r.ReadInt();
                MarketRuleIds = r.ReadString();
                MinSize = r.ReadDecimal();
                SizeIncrement = r.ReadDecimal();
                SuggestedSizeIncrement = r.ReadDecimal();
                break;

            case ContractDetailsType.ScannerContractType:
                Contract.ContractId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadStringEnum<ContractSecurityType>();
                Contract.LastTradeDateOrContractMonth = r.ReadString();
                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadStringEnum<ContractOptionRight>();
                Contract.Exchange = r.ReadString();
                Contract.Currency = r.ReadString();
                Contract.LocalSymbol = r.ReadString();
                MarketName = r.ReadString();
                Contract.TradingClass = r.ReadString();
                break;
        }

        if (RequestId == int.MaxValue)
            throw new InvalidDataException("Test Exception!!");
    }

    private void ReadLastTradeDate(string lastTradeDateOrContractMonth, bool isBond)
    {
        if (lastTradeDateOrContractMonth.Length == 0)
            return;

        string[] split = lastTradeDateOrContractMonth.Contains('-', StringComparison.Ordinal) ?
            lastTradeDateOrContractMonth.Split("-") :
            lastTradeDateOrContractMonth.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

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

[Message]
public sealed record ContractDetailsEnd : IHasRequestId
{
    public int RequestId { get; init; }
    internal ContractDetailsEnd() { }
    internal ContractDetailsEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
}
