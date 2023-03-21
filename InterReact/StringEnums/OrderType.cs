using StringEnums;

namespace InterReact;

public sealed class OrderType : StringEnum<OrderType>
{
    public static OrderType Undefined { get; } = Create("");

    public static OrderType Market { get; } = Create("MKT");
    public static OrderType MarketIfTouched { get; } = Create("MIT");
    public static OrderType MarketOnClose { get; } = Create("MOC");
    public static OrderType MarketOnOpen { get; } = Create("MOO");
    public static OrderType MarketToLimit { get; } = Create("MTL");
    public static OrderType MarketWithProtection { get; } = Create("MKT PRT");

    public static OrderType Limit { get; } = Create("LMT");
    public static OrderType LimitOnClose { get; } = Create("LOC");
    public static OrderType LimitOnOpen { get; } = Create("LOO");
    public static OrderType LimitIfTouched { get; } = Create("LIT");

    public static OrderType MidPrice { get; } = Create("MIDPRICE");

    public static OrderType Stop { get; } = Create("STP");
    public static OrderType StopLimit { get; } = Create("STP LMT");
    public static OrderType StopWithProtection { get; } = Create("STP PRT");
    public static OrderType TrailingStop { get; } = Create("TRAIL");
    public static OrderType TrailingStopLimit { get; } = Create("TRAIL LIMIT");
    public static OrderType TrailingLimitIfTouched { get; } = Create("TRAIL LIT");
    public static OrderType TrailingMarketIfTouched { get; } = Create("TRAIL MIT");

    public static OrderType PeggedToPrimary { get; } = Create("REL");
    public static OrderType PeggedToMarket { get; } = Create("PEG MKT");
    public static OrderType PeggedToBenchmark { get; } = Create("PEG BENCH");
    public static OrderType PeggedToBest { get; } = Create("PEG BEST");
    public static OrderType PeggedToMidpoint { get; } = Create("PEG MID");
    public static OrderType PeggedToStock { get; } = Create("PEG STK");

    public static OrderType GoodAfterTime { get; } = Create("GAT");
    public static OrderType GoodUntilDate { get; } = Create("GTD");
    public static OrderType GoodUntilCancelled { get; } = Create("GTC");
    public static OrderType ImmediateOrCancel { get; } = Create("IOC");
    public static OrderType OneCancelsAll { get; } = Create("OCA");

    public static OrderType RelativeLimitCombo { get; } = Create("REL + LMT");
    public static OrderType RelativeMarketCombo { get; } = Create("REL + MKT");
    public static OrderType PassiveRelative { get; } = Create("PASSV REL");

    public static OrderType BoxTop { get; } = Create("BOX TOP");
    public static OrderType VolumeWeightedAveragePrice { get; } = Create("VWAP");
    public static OrderType Volatility { get; } = Create("VOL");
    public static OrderType RequestForQuote { get; } = Create("QUOTE");
}
