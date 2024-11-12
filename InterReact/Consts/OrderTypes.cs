namespace InterReact;

public static class OrderTypes
{
    public static readonly string Undefined = "";

    public static readonly string Market = "MKT";
    public static readonly string MarketIfTouched = "MIT";
    public static readonly string MarketOnClose = "MOC";
    public static readonly string MarketOnOpen = "MOO";
    public static readonly string MarketToLimit = "MTL";
    public static readonly string MarketWithProtection = "MKT PRT";

    public static readonly string Limit = "LMT";
    public static readonly string LimitOnClose = "LOC";
    public static readonly string LimitOnOpen = "LOO";
    public static readonly string LimitIfTouched = "LIT";

    public static readonly string MidPrice = "MIDPRICE";

    public static readonly string Stop = "STP";
    public static readonly string StopLimit = "STP LMT";
    public static readonly string StopWithProtection = "STP PRT";
    public static readonly string TrailingStop = "TRAIL";
    public static readonly string TrailingStopLimit = "TRAIL LIMIT";
    public static readonly string TrailingLimitIfTouched = "TRAIL LIT";
    public static readonly string TrailingMarketIfTouched = "TRAIL MIT";

    public static readonly string PeggedToPrimary = "REL";
    public static readonly string PeggedToMarket = "PEG MKT";
    public static readonly string PeggedToBenchmark = "PEG BENCH";
    public static readonly string PeggedToBest = "PEG BEST";
    public static readonly string PeggedToMidpoint = "PEG MID";
    public static readonly string PeggedToStock = "PEG STK";

    public static readonly string GoodAfterTime = "GAT";
    public static readonly string GoodUntilDate = "GTD";
    public static readonly string GoodUntilCancelled = "GTC";
    public static readonly string ImmediateOrCancel = "IOC";
    public static readonly string OneCancelsAll = "OCA";

    public static readonly string RelativeLimitCombo = "REL + LMT";
    public static readonly string RelativeMarketCombo = "REL + MKT";
    public static readonly string PassiveRelative = "PASSV REL";

    public static readonly string BoxTop = "BOX TOP";
    public static readonly string VolumeWeightedAveragePrice = "VWAP";
    public static readonly string Volatility = "VOL";
    public static readonly string RequestForQuote = "QUOTE";
}
