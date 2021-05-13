using StringEnums;

namespace InterReact
{
    public sealed class HistoricalDataType : StringEnum<HistoricalDataType>
    {
        public static readonly HistoricalDataType Trades = Create("TRADES");
        public static readonly HistoricalDataType Midpoint = Create("MIDPOINT");
        public static readonly HistoricalDataType Bid = Create("BID");
        public static readonly HistoricalDataType Ask = Create("ASK");
        public static readonly HistoricalDataType BidAsk = Create("BID_ASK");
        public static readonly HistoricalDataType HistoricalVolatility = Create("HISTORICAL_VOLATILITY");
        public static readonly HistoricalDataType OptionImpliedVolatility = Create("OPTION_IMPLIED_VOLATILITY");
        public static readonly HistoricalDataType YieldBid = Create("YIELD_BID");
        public static readonly HistoricalDataType YieldAsk = Create("YIELD_ASK");
        public static readonly HistoricalDataType YieldBidAsk = Create("YIELD_BID_ASK");
        public static readonly HistoricalDataType YieldLast = Create("YIELD_LAST");
        public static readonly HistoricalDataType FeeRate = Create("FEE_RATE");
        public static readonly HistoricalDataType RebateRate = Create("REBATE_RATE");
    }

}

