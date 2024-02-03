namespace InterReact;

public static class OrderTypes
{
    public const string Undefined = "";

    public const string Market = "MKT";
    public const string MarketIfTouched = "MIT";
    public const string MarketOnClose = "MOC";
    public const string MarketOnOpen = "MOO";
    public const string MarketToLimit = "MTL";
    public const string MarketWithProtection = "MKT PRT";

    public const string Limit = "LMT";
    public const string LimitOnClose = "LOC";
    public const string LimitOnOpen = "LOO";
    public const string LimitIfTouched = "LIT";

    public const string MidPrice = "MIDPRICE";

    public const string Stop = "STP";
    public const string StopLimit = "STP LMT";
    public const string StopWithProtection = "STP PRT";
    public const string TrailingStop = "TRAIL";
    public const string TrailingStopLimit = "TRAIL LIMIT";
    public const string TrailingLimitIfTouched = "TRAIL LIT";
    public const string TrailingMarketIfTouched = "TRAIL MIT";

    public const string PeggedToPrimary = "REL";
    public const string PeggedToMarket = "PEG MKT";
    public const string PeggedToBenchmark = "PEG BENCH";
    public const string PeggedToBest = "PEG BEST";
    public const string PeggedToMidpoint = "PEG MID";
    public const string PeggedToStock = "PEG STK";

    public const string GoodAfterTime = "GAT";
    public const string GoodUntilDate = "GTD";
    public const string GoodUntilCancelled = "GTC";
    public const string ImmediateOrCancel = "IOC";
    public const string OneCancelsAll = "OCA";

    public const string RelativeLimitCombo = "REL + LMT";
    public const string RelativeMarketCombo = "REL + MKT";
    public const string PassiveRelative = "PASSV REL";

    public const string BoxTop = "BOX TOP";
    public const string VolumeWeightedAveragePrice = "VWAP";
    public const string Volatility = "VOL";
    public const string RequestForQuote = "QUOTE";
}
