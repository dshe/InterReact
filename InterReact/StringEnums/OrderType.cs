using StringEnums;

namespace InterReact;

public sealed class OrderType : StringEnum<OrderType>
{
    public static OrderType Undefined { get; } = Create("");

    public static OrderType Limit { get; } = Create("LMT");

    public static OrderType MarketToLimit { get; } = Create("MTL");

    public static OrderType MarketWithProtection { get; } = Create("MKT PRT");

    public static OrderType RequestForQuote { get; } = Create("QUOTE");

    /// <summary>
    /// A Stop order becomes a market order to buy or sell securities or commodities once the specified stop price is attained or penetrated.
    /// </summary>
    public static OrderType Stop { get; } = Create("STP");

    /// <summary>
    /// A STOP-LIMIT order is similar to a stop order in that a stop price will activate the order. However, once activated, the stop-limit order becomes a buy limit or sell limit order and can only be executed at a specific price or better. It is a combination of both the stop order and the limit order.
    /// </summary>
    public static OrderType StopLimit { get; } = Create("STP LMT");

    public static OrderType TrailingLimitIfTouched { get; } = Create("TRAIL LIT");
    public static OrderType TrailingMarketIfTouched { get; } = Create("TRAIL MIT");

    /// <summary>
    /// A trailing stop for a sell order sets the stop price at a fixed amount below the market price. If the market price rises, the stop loss price rises by the increased amount, but if the stock price falls, the stop loss price remains the same. The reverse is true for a buy trailing stop order.
    /// Forex, Futures, Future Options, Options, Stocks, Warrants.
    /// </summary>
    public static OrderType TrailingStop { get; } = Create("TRAIL");

    /// <summary>
    /// A trailing stop limit for a sell order sets the stop price at a fixed amount below the market price and defines a limit price for the sell order.
    /// If the market price rises, the stop loss price rises by the increased amount, but if the stock price falls, the stop loss price remains the same.
    /// When the order triggers, a limit order is submitted at the price you defined. The reverse is true for a buy trailing stop limit order.
    /// Forex, Futures, Future Options, Options, Stocks, Warrants.
    /// </summary>
    public static OrderType TrailingStopLimit { get; } = Create("TRAIL LIMIT");

    /// <summary>
    /// A Market order is an order to buy or sell an asset at the bid or offer price currently available in the marketplace.
    /// Bonds, Forex, Futures, Future Options, Options, Stocks, Warrants.
    /// </summary>
    public static OrderType Market { get; } = Create("MKT");

    public static OrderType MarketIfTouched { get; } = Create("MIT");

    /// <summary>
    /// A market order that is submitted to execute as close to the closing price as possible.
    /// Non US Futures, Non US Options, Stocks
    /// </summary>
    public static OrderType MarketOnClose { get; } = Create("MOC");

    public static OrderType MarketOnOpen { get; } = Create("MOO");

    /// <summary>
    /// An order that is pegged to buy on the best offer and sell on the best bid.
    /// Your order is pegged to buy on the best offer and sell on the best bid. 
    /// You can also use an offset amount which is subtracted from the best offer for a buy order, and added to the best bid for a sell order.
    /// </summary>
    public static OrderType PeggedToMarket { get; } = Create("PEG MKT");

    /// <summary>
    /// A Relative order derives its price from a combination of the market quote and a user-defined offset amount. The order is submitted as a limit order and modified according to the pricing logic until it is executed or you cancel the order.
    /// Options, Stocks.
    /// </summary>
    public static OrderType Relative { get; } = Create("REL");

    public static OrderType BoxTop { get; } = Create("BOX TOP");
    public static OrderType LimitOnClose { get; } = Create("LOC");
    public static OrderType LimitOnOpen { get; } = Create("LOO");
    public static OrderType LimitIfTouched { get; } = Create("LIT");
    public static OrderType PeggedToMidpoint { get; } = Create("PEG MID");

    /// <summary>
    /// The VWAP for a stock is calculated by adding the dollars traded for every transaction in that stock ("price" x "number of shares traded")
    /// and dividing the total shares traded. By default, a VWAP order is computed from the open of the market to the market close, and is calculated
    /// by volume weighting all transactions during this time period. TWS allows you to modify the cut-off and expiration times using the Time in
    /// Force and Expiration Date fields, respectively.
    /// </summary>
    public static OrderType VolumeWeightedAveragePrice { get; } = Create("VWAP");

    public static OrderType GoodAfterTime { get; } = Create("GAT");
    public static OrderType GoodUntilDate { get; } = Create("GTD");
    public static OrderType GoodUntilCancelled { get; } = Create("GTC");
    public static OrderType ImmediateOrCancel { get; } = Create("IOC");
    public static OrderType OneCancelsAll { get; } = Create("OCA");

    /// <summary>
    /// What happens with VOL orders is that the limit price that is sent to the exchange is computed by TWS as a function of a daily or annualized
    /// option volatility provided by the user. VOL orders can be placed for any US option that trades on the BOX exchange. VOL orders are
    /// eligible for dynamic management, a powerful new functionality wherein TWS can manage options orders in response to specifications set by the user.
    /// </summary>
    public static OrderType Volatility { get; } = Create("VOL");

    public static OrderType PeggedToBenchmark { get; } = Create("PEG BENCH");
}
