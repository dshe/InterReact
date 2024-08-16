using System.IO;
using System.Text.RegularExpressions;

namespace InterReact;

public sealed class ContractDetails : IHasRequestId
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

    //public int MdSizeMultiplier { get; }

    public int AggGroup { get; }

    /// <summary>
    /// CUSIP, ISIN etc.
    /// </summary>
    public IList<Tag> SecurityIds { get; } = [];

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
    public string NextOptionType { get; } = "";
    public bool NextOptionPartial { get; }
    public string Notes { get; } = "";
    public decimal MinSize { get; } = decimal.MaxValue;
    public decimal SizeIncrement { get; } = decimal.MaxValue;
    public decimal SuggestedSizeIncrement { get; } = decimal.MaxValue;
    public string FundName { get; } = "";
    public string FundFamily { get; } = "";
    public string FundType { get; } = "";
    public string FundFrontLoad { get; } = "";
    public string FundBackLoad { get; } = "";
    public string FundBackLoadTimeInterval { get; } = "";
    public string FundManagementFee { get; } = "";
    public bool FundClosed { get; }
    public bool FundClosedForNewInvestors { get; }
    public bool FundClosedForNewMoney { get; }
    public string FundNotifyAmount { get; } = "";
    public string FundMinimumInitialPurchase { get; } = "";
    public string FundSubsequentMinimumPurchase { get; } = "";
    public string FundBlueSkyStates { get; } = "";
    public string FundBlueSkyTerritories { get; } = "";
    public FundDistributionPolicyIndicator FundDistributionPolicyIndicator { get; }
    public FundAssetType FundAssetType { get; }
    public IList<IneligibilityReason> IneligibilityReasonList { get; } = [];
    internal ContractDetails(ResponseReader r, ContractDetailsType type)
    {
        switch (type)
        {
            case ContractDetailsType.GeneralContractType:
                RequestId = r.ReadInt();
                Contract.Symbol = r.ReadString();
                Contract.SecurityType = r.ReadString();

                ReadLastTradeDate(r.ReadString(), false);
                //if (r.Options.ServerVersionCurrent >= ServerVersion.LAST_TRADE_DATE)
                //    Contract.LastTradeDateOrContractMonth = r.ReadString(); // ???

                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadString();
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
                */

                /*
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
                Contract.SecurityType = r.ReadString();
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
                Contract.SecurityType = r.ReadString();
                Contract.LastTradeDateOrContractMonth = r.ReadString();
                Contract.Strike = r.ReadDouble();
                Contract.Right = r.ReadString();
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

        string[] split = lastTradeDateOrContractMonth.Contains('-', StringComparison.Ordinal) ? Regex.Split(lastTradeDateOrContractMonth, "-") : Regex.Split(lastTradeDateOrContractMonth, "\\s+");
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
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
}
