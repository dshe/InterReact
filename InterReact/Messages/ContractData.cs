using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    enum ContractDataType { ContractData, BondContractData, ScannerContractData }

    public sealed class ContractData : IHasRequestId // output
    {
        /// <summary>
        /// The Id that was used to make the request to receive this ContractData.
        /// </summary>
        public int RequestId { get; private set; }

        public Contract Contract { get; private set; }

        public string MarketName { get; private set; } = "";
        public double MinimumTick { get; private set; }

        /// <summary>
        /// Allows execution and strike prices to be reported consistently with market data, historical data and the order price,
        /// i.e. Z on LIFFE is reported in index points and not GBP.
        /// </summary>
        public int PriceMagnifier { get; private set; }

        /// <summary>
        /// The list of valid order types for this contract.
        /// </summary>
        public string OrderTypes { get; private set; } = "";

        /// <summary>
        /// The list of exchanges this contract is traded on.
        /// </summary>
        public string ValidExchanges { get; private set; } = "";

        public int UnderlyingContractId { get; private set; }

        public string LongName { get; private set; } = "";

        /// <summary>
        /// The contract month. Typically the contract month of the underlying for a futures contract.
        /// </summary>
        public string ContractMonth { get; private set; } = "";

        /// <summary>
        /// The industry classification of the underlying/product. 
        /// </summary>
        public string Industry { get; private set; } = "";

        /// <summary>
        /// The industry category of the underlying.
        /// </summary>
        public string Category { get; private set; } = "";

        public string Subcategory { get; private set; } = "";

        /// <summary>
        /// The ID of the time zone for the trading hours of the product. For example, EST.
        /// </summary>
        public string TimeZoneId { get; private set; } = "";

        /// <summary>
        /// The total trading hours of the product. For example, 20090507:0700-1830,1830-2330;20090508:CLOSED.
        /// </summary>
        public string TradingHours { get; private set; } = "";

        /// <summary>
        /// The liquid trading hours of the product. For example, 20090507:0930-1600;20090508:CLOSED.
        /// </summary>
        public string LiquidHours { get; private set; } = "";

        /// <summary>
        /// For products in Australia which trade in non-currency units.
        /// This string attribute contains the Economic Value Rule name and the respective optional argument.
        /// The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. 
        /// When the optional argument is not present, the first value will be followed by a colon.
        /// </summary>
        public string EconomicValueRule { get; private set; } = "";

        /// <summary>
        /// For products in Australia which trade in non-currency units.
        /// This double attribute tells you approximately how much the market value of a contract would change if the price were to change by 1.
        /// It cannot be used to get market value by multiplying the price by the approximate multiplier.
        /// </summary>
        public double EconomicValueMultiplier { get; private set; }

        public int MdSizeMultiplier { get; private set; }

        public int AggGroup { get; private set; }

        /// <summary>
        /// CUSIP, ISIN etc.
        /// </summary>
        public IList<Tag> SecurityIds { get; } = new List<Tag>(); // output

        public string UnderSymbol { get; private set; } = "";
        public string UnderSecType { get; private set; } = "";
        public string MarketRuleIds { get; private set; } = "";
        public string RealExpirationDate { get; private set; } = "";
        public string LastTradeTime { get; private set; } = "";


        // BOND Values

        /// <summary>
        /// The nine-character bond CUSIP or the 12-character SEDOL.
        /// </summary>
        public string Cusip { get; private set; } = "";
        public string CreditRatings { get; private set; } = "";
        public string DescriptionAppend { get; private set; } = "";
        public string BondType { get; private set; } = "";
        public string CouponType { get; private set; } = "";
        public bool Callable { get; private set; }
        public bool Putable { get; private set; }
        public double Coupon { get; private set; }
        public bool Convertible { get; private set; }
        public string Maturity { get; private set; } = "";
        public string IssueDate { get; private set; } = "";
        public string NextOptionDate { get; private set; } = "";

        /// <summary>
        /// If bond has embedded options.
        /// </summary>
        public string NextOptionType { get; private set; } = "";

        /// <summary>
        /// If bond has embedded options.
        /// </summary>
        public bool NextOptionPartial { get; private set; }
        
        public string Notes { get; private set; } = "";

        internal ContractData() => Contract = new Contract(); // ctor for testing

        internal ContractData(ResponseComposer c, ContractDataType type)
        {
            switch (type)
            {
                case ContractDataType.ContractData:
                    c.RequireVersion(8);
                    RequestId = c.ReadInt();
                    Contract = new Contract
                    {
                        Symbol = c.ReadString(),
                        SecurityType = c.ReadStringEnum<SecurityType>(),
                    };

                    ReadLastTradeDate(c, false);

                    Contract.Strike = c.ReadDouble();
                    Contract.Right = c.ReadStringEnum<RightType>();
                    Contract.Exchange = c.ReadString();
                    Contract.Currency = c.ReadString();
                    Contract.LocalSymbol = c.ReadString();
                    MarketName = c.ReadString();
                    Contract.TradingClass = c.ReadString();
                    Contract.ContractId = c.ReadInt();
                    MinimumTick = c.ReadDouble();
                    if (c.Config.SupportsServerVersion(ServerVersion.MdSizeMultiplier))
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
                    if (c.Config.SupportsServerVersion(ServerVersion.AggGroup))
                        AggGroup = c.ReadInt();
                    if (c.Config.SupportsServerVersion(ServerVersion.UnderlyingInfo))
                    {
                        UnderSymbol = c.ReadString();
                        UnderSecType = c.ReadString();
                    }
                    if (c.Config.SupportsServerVersion(ServerVersion.MarketRules))
                        MarketRuleIds = c.ReadString();
                    if (c.Config.SupportsServerVersion(ServerVersion.RealExpirationDate))
                        RealExpirationDate = c.ReadString();
                    break;
                case ContractDataType.BondContractData:
                    var version = c.RequireVersion(6);
                    RequestId = c.ReadInt();
                    Contract = new Contract
                    {
                        Symbol = c.ReadString(),
                        SecurityType = c.ReadStringEnum<SecurityType>()
                    };
                    Cusip = c.ReadString();
                    Coupon = c.ReadDouble();

                    ReadLastTradeDate(c, true);

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
                    if (c.Config.SupportsServerVersion(ServerVersion.MdSizeMultiplier))
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
                    if (c.Config.SupportsServerVersion(ServerVersion.AggGroup))
                        AggGroup = c.ReadInt();
                    if (c.Config.SupportsServerVersion(ServerVersion.MarketRules))
                        MarketRuleIds = c.ReadString();
                    break;
                case ContractDataType.ScannerContractData:
                    Contract = new Contract
                    {
                        ContractId = c.ReadInt(),
                        Symbol = c.ReadString(),
                        SecurityType = c.ReadStringEnum<SecurityType>(),
                        LastTradeDateOrContractMonth = c.ReadString(),
                        Strike = c.ReadDouble(),
                        Right = c.ReadStringEnum<RightType>(),
                        Exchange = c.ReadString(),
                        Currency = c.ReadString(),
                        LocalSymbol = c.ReadString(),
                    };
                    MarketName = c.ReadString();
                    Contract.TradingClass = c.ReadString();
                    break;
                default:
                    throw new Exception("Invalid ContractDataType");
            }
        }

        private void ReadLastTradeDate(ResponseComposer c, bool isBond)
        {
            string lastTradeDateOrContractMonth = c.ReadString();
            if (lastTradeDateOrContractMonth == null)
                return;
            string[] splitted = Regex.Split(lastTradeDateOrContractMonth, "\\s+");
            if (splitted.Length > 0)
            {
                if (isBond)
                    Maturity = splitted[0];
                else
                    Contract.LastTradeDateOrContractMonth = splitted[0];
            }
            if (splitted.Length > 1)
                LastTradeTime = splitted[1];
            if (isBond && splitted.Length > 2)
                TimeZoneId = splitted[2];
        }
    }

    public sealed class ContractDataEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal ContractDataEnd(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }

    }
}
