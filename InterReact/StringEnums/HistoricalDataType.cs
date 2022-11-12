using StringEnums;

namespace InterReact;

public sealed class HistoricalDataType : StringEnum<HistoricalDataType>
{
    public static HistoricalDataType Trades { get; } = Create("TRADES");
    public static HistoricalDataType Midpoint { get; } = Create("MIDPOINT");
    public static HistoricalDataType Bid { get; } = Create("BID");
    public static HistoricalDataType Ask { get; } = Create("ASK");
    public static HistoricalDataType BidAsk { get; } = Create("BID_ASK");
    public static HistoricalDataType HistoricalVolatility { get; } = Create("HISTORICAL_VOLATILITY");
    public static HistoricalDataType OptionImpliedVolatility { get; } = Create("OPTION_IMPLIED_VOLATILITY");
    public static HistoricalDataType YieldBid { get; } = Create("YIELD_BID");
    public static HistoricalDataType YieldAsk { get; } = Create("YIELD_ASK");
    public static HistoricalDataType YieldBidAsk { get; } = Create("YIELD_BID_ASK");
    public static HistoricalDataType YieldLast { get; } = Create("YIELD_LAST");
    public static HistoricalDataType FeeRate { get; } = Create("FEE_RATE");
    public static HistoricalDataType RebateRate { get; } = Create("REBATE_RATE");
}

