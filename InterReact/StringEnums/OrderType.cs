using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class OrderType : StringEnum<OrderType>
    {
        public static readonly OrderType Undefined = Create("");

        public static readonly OrderType Limit = Create("LMT");

        public static readonly OrderType MarketToLimit = Create("MTL");

        public static readonly OrderType MarketWithProtection = Create("MKT PRT");

        public static readonly OrderType RequestForQuote = Create("QUOTE");

        /// <summary>
        /// A Stop order becomes a market order to buy or sell securities or commodities once the specified stop price is attained or penetrated.
        /// </summary>
        public static readonly OrderType Stop = Create("STP");

        /// <summary>
        /// A STOP-LIMIT order is similar to a stop order in that a stop price will activate the order. However, once activated, the stop-limit order becomes a buy limit or sell limit order and can only be executed at a specific price or better. It is a combination of both the stop order and the limit order.
        /// </summary>
        public static readonly OrderType StopLimit = Create("STP LMT");

        public static readonly OrderType TrailingLimitIfTouched = Create("TRAIL LIT");
        public static readonly OrderType TrailingMarketIfTouched = Create("TRAIL MIT");

        /// <summary>
        /// A trailing stop for a sell order sets the stop price at a fixed amount below the market price. If the market price rises, the stop loss price rises by the increased amount, but if the stock price falls, the stop loss price remains the same. The reverse is true for a buy trailing stop order.
        /// Forex, Futures, Future Options, Options, Stocks, Warrants.
        /// </summary>
        public static readonly OrderType TrailingStop = Create("TRAIL");

        /// <summary>
        /// A trailing stop limit for a sell order sets the stop price at a fixed amount below the market price and defines a limit price for the sell order.
        /// If the market price rises, the stop loss price rises by the increased amount, but if the stock price falls, the stop loss price remains the same.
        /// When the order triggers, a limit order is submitted at the price you defined. The reverse is true for a buy trailing stop limit order.
        /// Forex, Futures, Future Options, Options, Stocks, Warrants.
        /// </summary>
        public static readonly OrderType TrailingStopLimit = Create("TRAIL LIMIT");

        /// <summary>
        /// A Market order is an order to buy or sell an asset at the bid or offer price currently available in the marketplace.
        /// Bonds, Forex, Futures, Future Options, Options, Stocks, Warrants.
        /// </summary>
        public static readonly OrderType Market = Create("MKT");

        public static readonly OrderType MarketIfTouched = Create("MIT");

        /// <summary>
        /// A market order that is submitted to execute as close to the closing price as possible.
        /// Non US Futures, Non US Options, Stocks
        /// </summary>
        public static readonly OrderType MarketOnClose = Create("MOC");

        public static readonly OrderType MarketOnOpen = Create("MOO");

        /// <summary>
        /// An order that is pegged to buy on the best offer and sell on the best bid.
        /// Your order is pegged to buy on the best offer and sell on the best bid. 
        /// You can also use an offset amount which is subtracted from the best offer for a buy order, and added to the best bid for a sell order.
        /// </summary>
        public static readonly OrderType PeggedToMarket = Create("PEG MKT");

        /// <summary>
        /// A Relative order derives its price from a combination of the market quote and a user-defined offset amount. The order is submitted as a limit order and modified according to the pricing logic until it is executed or you cancel the order.
        /// Options, Stocks.
        /// </summary>
        public static readonly OrderType Relative = Create("REL");

        public static readonly OrderType BoxTop = Create("BOX TOP");
        public static readonly OrderType LimitOnClose = Create("LOC");
        public static readonly OrderType LimitOnOpen = Create("LOO");
        public static readonly OrderType LimitIfTouched = Create("LIT");
        public static readonly OrderType PeggedToMidpoint = Create("PEG MID");

        /// <summary>
        /// The VWAP for a stock is calculated by adding the dollars traded for every transaction in that stock ("price" x "number of shares traded")
        /// and dividing the total shares traded. By default, a VWAP order is computed from the open of the market to the market close, and is calculated
        /// by volume weighting all transactions during this time period. TWS allows you to modify the cut-off and expiration times using the Time in
        /// Force and Expiration Date fields, respectively.
        /// </summary>
        public static readonly OrderType VolumeWeightedAveragePrice = Create("VWAP");

        public static readonly OrderType GoodAfterTime = Create("GAT");
        public static readonly OrderType GoodUntilDate = Create("GTD");
        public static readonly OrderType GoodUntilCancelled = Create("GTC");
        public static readonly OrderType ImmediateOrCancel = Create("IOC");
        public static readonly OrderType OneCancelsAll = Create("OCA");

        /// <summary>
        /// What happens with VOL orders is that the limit price that is sent to the exchange is computed by TWS as a function of a daily or annualized
        /// option volatility provided by the user. VOL orders can be placed for any US option that trades on the BOX exchange. VOL orders are
        /// eligible for dynamic management, a powerful new functionality wherein TWS can manage options orders in response to specifications set by the user.
        /// </summary>
        public static readonly OrderType Volatility = Create("VOL");

        public static readonly OrderType PeggedToBenchmark = Create("PEG BENCH");
    }

}