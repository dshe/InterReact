namespace InterReact;

public sealed record OrderTypes(string StringCode) : IHasStringCode
{
    public static readonly OrderTypes Undefined = new("");

    public static readonly OrderTypes Market = new("MKT");
    public static readonly OrderTypes MarketIfTouched = new("MIT");
    public static readonly OrderTypes MarketOnClose = new("MOC");
    public static readonly OrderTypes MarketOnOpen = new("MOO");
    public static readonly OrderTypes MarketToLimit = new("MTL");
    public static readonly OrderTypes MarketWithProtection = new("MKT PRT");

    public static readonly OrderTypes Limit = new("LMT");
    public static readonly OrderTypes LimitOnClose = new("LOC");
    public static readonly OrderTypes LimitOnOpen = new("LOO");
    public static readonly OrderTypes LimitIfTouched = new("LIT");

    public static readonly OrderTypes MidPrice = new("MIDPRICE");

    public static readonly OrderTypes Stop = new("STP");
    public static readonly OrderTypes StopLimit = new("STP LMT");
    public static readonly OrderTypes StopWithProtection = new("STP PRT");
    public static readonly OrderTypes TrailingStop = new("TRAIL");
    public static readonly OrderTypes TrailingStopLimit = new("TRAIL LIMIT");
    public static readonly OrderTypes TrailingLimitIfTouched = new("TRAIL LIT");
    public static readonly OrderTypes TrailingMarketIfTouched = new("TRAIL MIT");

    public static readonly OrderTypes PeggedToPrimary = new("REL");
    public static readonly OrderTypes PeggedToMarket = new("PEG MKT");
    public static readonly OrderTypes PeggedToBenchmark = new("PEG BENCH");
    public static readonly OrderTypes PeggedToBest = new("PEG BEST");
    public static readonly OrderTypes PeggedToMidpoint = new("PEG MID");
    public static readonly OrderTypes PeggedToStock = new("PEG STK");

    public static readonly OrderTypes GoodAfterTime = new("GAT");
    public static readonly OrderTypes GoodUntilDate = new("GTD");
    public static readonly OrderTypes GoodUntilCancelled = new("GTC");
    public static readonly OrderTypes ImmediateOrCancel = new("IOC");
    public static readonly OrderTypes OneCancelsAll = new("OCA");

    public static readonly OrderTypes RelativeLimitCombo = new("REL + LMT");
    public static readonly OrderTypes RelativeMarketCombo = new("REL + MKT");
    public static readonly OrderTypes PassiveRelative = new("PASSV REL");

    public static readonly OrderTypes BoxTop = new("BOX TOP");
    public static readonly OrderTypes VolumeWeightedAveragePrice = new("VWAP");
    public static readonly OrderTypes Volatility = new("VOL");
    public static readonly OrderTypes RequestForQuote = new("QUOTE");
}
