using System.Collections.Generic;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class ContractDetails : IHasRequestId // output
    {
        /// <summary>
        /// The Id that was used to make the request to receive this ContractDetails.
        /// </summary>
        public int RequestId { get; internal set; }

        public Contract Contract { get; } = new Contract();

        public string MarketName { get; internal set; }
        public double MinimumTick { get; internal set; }

        /// <summary>
        /// Allows execution and strike prices to be reported consistently with market data, historical data and the order price,
        /// i.e. Z on LIFFE is reported in index points and not GBP.
        /// </summary>
        public int PriceMagnifier { get; internal set; }

        /// <summary>
        /// The list of valid order types for this contract.
        /// </summary>
        public string OrderTypes { get; internal set; }

        /// <summary>
        /// The list of exchanges this contract is traded on.
        /// </summary>
        public string ValidExchanges { get; internal set; }

        public int UnderlyingContractId { get; internal set; }

        public string LongName { get; internal set; }

        /// <summary>
        /// The contract month. Typically the contract month of the underlying for a futures contract.
        /// </summary>
        public string ContractMonth { get; internal set; }

        /// <summary>
        /// The industry classification of the underlying/product. 
        /// </summary>
        public string Industry { get; internal set; }

        /// <summary>
        /// The industry category of the underlying.
        /// </summary>
        public string Category { get; internal set; }

        public string Subcategory { get; internal set; }

        /// <summary>
        /// The ID of the time zone for the trading hours of the product. For example, EST.
        /// </summary>
        public string TimeZoneId { get; internal set; }

        /// <summary>
        /// The total trading hours of the product. For example, 20090507:0700-1830,1830-2330;20090508:CLOSED.
        /// </summary>
        public string TradingHours { get; internal set; }

        /// <summary>
        /// The liquid trading hours of the product. For example, 20090507:0930-1600;20090508:CLOSED.
        /// </summary>
        public string LiquidHours { get; internal set; }

        /// <summary>
        /// For products in Australia which trade in non-currency units.
        /// This string attribute contains the Economic Value Rule name and the respective optional argument.
        /// The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. 
        /// When the optional argument is not present, the first value will be followed by a colon.
        /// </summary>
        public string EconomicValueRule { get; internal set; }

        /// <summary>
        /// For products in Australia which trade in non-currency units.
        /// This double attribute tells you approximately how much the market value of a contract would change if the price were to change by 1.
        /// It cannot be used to get market value by multiplying the price by the approximate multiplier.
        /// </summary>
        public double EconomicValueMultiplier { get; internal set; }

        public int MdSizeMultiplier { get; internal set; }

        /// <summary>
        /// CUSIP, ISIN etc.
        /// </summary>
        public IReadOnlyList<Tag> SecurityIds { get; internal set; } = new List<Tag>(); // output

        // BOND Values

        /// <summary>
        /// The nine-character bond CUSIP or the 12-character SEDOL.
        /// </summary>
        public string Cusip { get; internal set; }
        public string CreditRatings { get; internal set; }
        public string DescriptionAppend { get; internal set; }
        public string BondType { get; internal set; }
        public string CouponType { get; internal set; }
        public bool Callable { get; internal set; }
        public bool Putable { get; internal set; }
        public double Coupon { get; internal set; }
        public bool Convertible { get; internal set; }
        public string Maturity { get; internal set; }
        public string IssueDate { get; internal set; }
        public string NextOptionDate { get; internal set; }

        /// <summary>
        /// If bond has embedded options.
        /// </summary>
        public string NextOptionType { get; internal set; }

        /// <summary>
        /// If bond has embedded options.
        /// </summary>
        public bool NextOptionPartial { get; internal set; }
        
        public string Notes { get; internal set; }
    }


    public sealed class ContractDetailsEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
