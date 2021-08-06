using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InterReact
{
    internal enum ContractDetailsType { ContractData, BondContractData, ScannerContractData }

    public sealed class ContractDetails : IHasRequestId // output
    {
        /// <summary>
        /// The Id that was used to make the request to receive this ContractData.
        /// </summary>
        public int RequestId { get; }

        public Contract Contract { get; } = new Contract();

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
        public List<Tag> SecurityIds { get; } = new List<Tag>(); // output

        public string UnderSymbol { get; } = "";
        public string UnderSecType { get; } = "";
        public string MarketRuleIds { get; } = "";
        public string RealExpirationDate { get; } = "";
        public string LastTradeTime { get; private set; } = "";


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

        internal ContractDetails() { } // ctor for testing

        internal ContractDetails(ResponseReader c, ContractDetailsType type)
        {
            switch (type)
            {
                case ContractDetailsType.ContractData:
                    c.RequireVersion(8);
                    RequestId = c.ReadInt();

                    Contract.Symbol = c.ReadString();
                    Contract.SecurityType = c.ReadStringEnum<SecurityType>();

                    ReadLastTradeDate(c.ReadString(), false);

                    Contract.Strike = c.ReadDouble();
                    Contract.Right = c.ReadStringEnum<OptionRightType>();
                    Contract.Exchange = c.ReadString();
                    Contract.Currency = c.ReadString();
                    Contract.LocalSymbol = c.ReadString();

                    MarketName = c.ReadString();

                    Contract.TradingClass = c.ReadString();
                    Contract.ContractId = c.ReadInt();

                    MinimumTick = c.ReadDouble();
                    if (c.Config.SupportsServerVersion(ServerVersion.MD_SIZE_MULTIPLIER))
                        MdSizeMultiplier = c.ReadInt();

                    Contract.Multiplier = c.ReadString();

                    OrderTypes = c.ReadString();
                    ValidExchanges = c.ReadString();
                    PriceMagnifier = c.ReadInt();
                    UnderlyingContractId = c.ReadInt();
                    LongName = c.ReadString();

                    Contract.PrimaryExchange = c.ReadString();

                    ContractMonth = c.ReadString();
                    Industry = c.ReadString();
                    Category = c.ReadString();
                    Subcategory = c.ReadString();
                    TimeZoneId = c.ReadString();
                    TradingHours = c.ReadString();
                    LiquidHours = c.ReadString();
                    EconomicValueRule = c.ReadString();
                    EconomicValueMultiplier = c.ReadDouble();
                    c.AddTagsToList(SecurityIds);
                    if (c.Config.SupportsServerVersion(ServerVersion.AGG_GROUP))
                        AggGroup = c.ReadInt();
                    if (c.Config.SupportsServerVersion(ServerVersion.UNDERLYING_INFO))
                    {
                        UnderSymbol = c.ReadString();
                        UnderSecType = c.ReadString();
                    }
                    break;

                case ContractDetailsType.BondContractData:
                    int version = c.RequireVersion(6);
                    RequestId = c.ReadInt();

                    Contract.Symbol = c.ReadString();
                    Contract.SecurityType = c.ReadStringEnum<SecurityType>();

                    Cusip = c.ReadString();
                    Coupon = c.ReadDouble();

                    ReadLastTradeDate(c.ReadString(), true);

                    IssueDate = c.ReadString();
                    CreditRatings = c.ReadString();
                    BondType = c.ReadString();
                    CouponType = c.ReadString();
                    Convertible = c.ReadBool();
                    Callable = c.ReadBool();
                    Putable = c.ReadBool();
                    DescriptionAppend = c.ReadString();

                    Contract.Exchange = c.ReadString();
                    Contract.Currency = c.ReadString();
                    MarketName = c.ReadString();
                    Contract.TradingClass = c.ReadString();
                    Contract.ContractId = c.ReadInt();
                    MinimumTick = c.ReadDouble();
                    if (c.Config.SupportsServerVersion(ServerVersion.MD_SIZE_MULTIPLIER))
                        MdSizeMultiplier = c.ReadInt();
                    OrderTypes = c.ReadString();
                    ValidExchanges = c.ReadString();
                    NextOptionDate = c.ReadString();
                    NextOptionType = c.ReadString();
                    NextOptionPartial = c.ReadBool();
                    Notes = c.ReadString();
                    LongName = c.ReadString();
                    if (version >= 6)
                    {
                        EconomicValueRule = c.ReadString();
                        EconomicValueMultiplier = c.ReadDouble();
                    }
                    if (version >= 5)
                        c.AddTagsToList(SecurityIds);
                    if (c.Config.SupportsServerVersion(ServerVersion.AGG_GROUP))
                        AggGroup = c.ReadInt();
                    break;

                case ContractDetailsType.ScannerContractData:
                    Contract.ContractId = c.ReadInt();
                    Contract.Symbol = c.ReadString();
                    Contract.SecurityType = c.ReadStringEnum<SecurityType>();
                    Contract.LastTradeDateOrContractMonth = c.ReadString();
                    Contract.Strike = c.ReadDouble();
                    Contract.Right = c.ReadStringEnum<OptionRightType>();
                    Contract.Exchange = c.ReadString();
                    Contract.Currency = c.ReadString();
                    Contract.LocalSymbol = c.ReadString();
                    MarketName = c.ReadString();
                    Contract.TradingClass = c.ReadString();
                    break;
            }
        }

        private void ReadLastTradeDate(string lastTradeDateOrContractMonth, bool isBond)
        {
            if (lastTradeDateOrContractMonth == "")
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
        internal ContractDetailsEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }
    }
}
