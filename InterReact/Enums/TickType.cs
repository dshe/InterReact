namespace InterReact;

public enum TickType
{
    /// <summary> 
    /// This value is used as the default to distinguish it from BidSize = 0
    /// </summary>
    Undefined = -2,

    /// <summary> 
    /// from TickMarketDataType. This value is used internally.
    /// </summary>
    MarketDataType = -1,

    /// <summary>
    /// Bid Size, from TickSize
    /// </summary>
    BidSize = 0,

    /// <summary>
    /// Bid Price, from TickPrice
    /// </summary>
    BidPrice = 1,

    /// <summary>
    /// Ask Price, from TickPrice
    /// </summary>
    AskPrice = 2,

    /// <summary>
    /// Ask Size, from TickSize
    /// </summary>
    AskSize = 3,

    /// <summary>
    /// Last Price, from TickPrice
    /// </summary>
    LastPrice = 4,

    /// <summary>
    /// Last Size, from TickSize
    /// </summary>
    LastSize = 5,

    /// <summary>
    /// High Price, from TickPrice
    /// </summary>
    HighPrice = 6,

    /// <summary>
    /// Low Price, from TickPrice
    /// </summary>
    LowPrice = 7,

    /// <summary>
    /// Volume, from TickSize
    /// </summary>
    Volume = 8,

    /// <summary>
    /// Close Price, from TickPrice
    /// </summary>
    ClosePrice = 9,

    /// <summary>
    /// Option Bid, from TickOption
    /// </summary>
    BidOptionComputation = 10,

    /// <summary>
    /// Option Ask, from TickOption
    /// </summary>
    AskOptionComputation = 11,

    /// <summary>
    /// Option Last, from TickOption
    /// </summary>
    LastOptionComputation = 12,

    /// <summary>
    /// Model Option, from TickOption
    /// </summary>
    ModelOptionComputation = 13,

    /// <summary>
    /// Open Price, from TickPrice
    /// </summary>
    OpenPrice = 14,

    /// <summary>
    /// Low Price over last 13 weeks, from TickPrice
    /// </summary>
    Low13Week = 15,

    /// <summary>
    /// High Price over last 13 weeks, from TickPrice
    /// </summary>
    High13Week = 16,

    /// <summary>
    /// Low Price over last 26 weeks, from TickPrice
    /// </summary>
    Low26Week = 17,

    /// <summary>
    /// High Price over last 26 weeks, from TickPrice
    /// </summary>
    High26Week = 18,

    /// <summary>
    /// Low Price over last 52 weeks, from TickPrice
    /// </summary>
    Low52Week = 19,

    /// <summary>
    /// High Price over last 52 weeks, from TickPrice
    /// </summary>
    High52Week = 20,

    /// <summary>
    /// Average Volume, from TickSize
    /// </summary>
    AverageVolume = 21,

    /// <summary>
    /// Open Interest, from TickSize
    /// </summary>
    OpenInterest = 22,

    /// <summary>
    /// Option Historical Volatility, from TickGeneric
    /// </summary>
    OptionHistoricalVolatility = 23,

    /// <summary>
    /// Option Implied Volatility, from TickGeneric
    /// </summary>
    OptionImpliedVolatility = 24,

    /// <summary>
    /// Option Bid Exchange (not used(?))
    /// </summary>
    OptionBidExchange = 25,

    /// <summary>
    /// Option Ask Exchange (not used(?))
    /// </summary>
    OptionAskExchange = 26,

    /// <summary>
    /// Option Call Open Interest, from TickSize
    /// </summary>
    OptionCallOpenInterest = 27,

    /// <summary>
    /// Option Put Open Interest, from TickSize
    /// </summary>
    OptionPutOpenInterest = 28,

    /// <summary>
    /// Option Call Volume, from TickSize
    /// </summary>
    OptionCallVolume = 29,

    /// <summary>
    /// Option Put Volume, from TickSize
    /// </summary>
    OptionPutVolume = 30,

    /// <summary>
    /// Index Future Premium, from TickGeneric
    /// </summary>
    IndexFuturePremium = 31,

    /// <summary>
    /// Bid Exchange, from TickString
    /// </summary>
    BidExchange = 32,

    /// <summary>
    /// Ask Exchange, from TickString
    /// </summary>
    AskExchange = 33,

    /// <summary>
    /// Auction Volume, from TickSize
    /// </summary>
    AuctionVolume = 34,

    /// <summary>
    /// Auction Price, from TickPrice
    /// </summary>
    AuctionPrice = 35,

    /// <summary>
    /// Auction Imbalance, from TickSize
    /// </summary>
    AuctionImbalance = 36,

    /// <summary>
    /// Mark Price, from TickPrice
    /// </summary>
    MarkPrice = 37,

    /// <summary>
    /// Bid EFP Computation, from TickExchangeForPhysical
    /// </summary>
    BidExchangeForPhysicalComputation = 38,

    /// <summary>
    /// Ask EFP Computation, from TickExchangeForPhysical
    /// </summary>
    AskExchangeForPhysicalComputation = 39,

    /// <summary>
    /// Last EFP Computation, from TickExchangeForPhysical
    /// </summary>
    LastExchangeForPhysicalComputation = 40,

    /// <summary>
    /// Open EFP Computation, from TickExchangeForPhysical
    /// </summary>
    OpenExchangeForPhysicalComputation = 41,

    /// <summary>
    /// High EFP Computation, from TickExchangeForPhysical
    /// </summary>
    HighExchangeForPhysicalComputation = 42,

    /// <summary>
    /// Low EFP Computation, from TickExchangeForPhysical
    /// </summary>
    LowExchangeForPhysicalComputation = 43,

    /// <summary>
    /// Close EFP Computation, from TickExchangeForPhysical
    /// </summary>
    CloseExchangeForPhysicalComputation = 44,

    /// <summary>
    /// Last time stamp, from TickString
    /// </summary>
    LastTimeStamp = 45,

    /// <summary>
    /// from TickString
    /// </summary>
    Shortable = 46,

    /// <summary>
    /// from TickString
    /// </summary>
    FundamentalRatios = 47,

    /// <summary>
    /// from TickGeneric
    /// </summary>
    RealtimeVolume = 48,

    /// <summary>
    /// Trading Halt status; sent whenever the TickHalt changes.
    ///</summary>
    Halted = 49,

    /// <summary>
    /// Bond Yield for Bid Price, from TickPrice
    /// </summary>
    BidYield = 50,

    /// <summary>
    /// Bond Yield for Ask Price, from TickPrice
    /// </summary>
    AskYield = 51,

    /// <summary>
    /// Bond Yield for Last Price, from TickPrice
    /// </summary>
    LastYield = 52,

    /// <summary>
    /// Implied Volatility, from TickOption, resulting from ImpliedVolatilityCalculate() request.
    /// </summary>
    CustomOptionComputation = 53,

    /// <summary>
    /// Trades, from TickGeneric
    /// </summary>
    TradeCount = 54,

    /// <summary>
    /// Trades per Minute, from TickGeneric
    /// </summary>
    TradeRate = 55,

    /// <summary>
    /// Volume per Minute, from TickGeneric
    /// </summary>
    VolumeRate = 56,

    /// <summary>
    /// The last trade during regular trading hours.
    /// </summary>
    LastRegularTradingHoursTrade = 57,
    RealtimeHistoricalVolatility = 58,
    IbDividends = 59,
    BondFactorMultiplier = 60,
    RegulatoryImbalance = 61,
    NewsTick = 62,
    ShortTermVolume3Min = 63,
    ShortTermVolume5Min = 64,
    ShortTermVolume10Min = 65,
    DelayedBidPrice = 66,
    DelayedAskPrice = 67,
    DelayedLastPrice = 68,
    DelayedBidSize = 69,
    DelayedAskSize = 70,
    DelayedLastSize = 71,
    DelayedHighPrice = 72,
    DelayedLowPrice = 73,
    DelayedVolume = 74,
    DelayedClosePrice = 75,
    DelayedOpenPrice = 76,
    RtTrdVolume = 77,
    CreditmanMarkPrice = 78,
    CreditmanSlowMarkPrice = 79,
    DelayedBidOptionComputation = 80,
    DelayedAskOptionComputation = 81,
    DelayedLastOptionComputation = 82,
    DelayedModelOptionComputation = 83,
    LastExchange = 84,
    LastRegTime = 85,
    FuturesOpenInterest = 86,
    AvgOptVolume = 87,
    DelayedLastTimeStamp = 88,
    ShortableShares = 89,
    DelayedHalted = 90,
    Reuters2MutualFunds = 91,
    EtfNavClose = 92,
    EtfNavPriorClose = 93,
    EtfNavBid = 94,
    EtfNavAsk = 95,
    EtfNavLast = 96,
    EtfFrozenNavLast = 97,
    EtfNavHigh = 98,
    EtfNavLow = 99
}

public static partial class Ext
{
    public static string ToStringFast(this TickType tickType)
    {
        return tickType switch
        {
            TickType.BidSize => nameof(TickType.BidSize),
            TickType.BidPrice => nameof(TickType.BidPrice),
            TickType.AskPrice => nameof(TickType.AskPrice),
            TickType.AskSize => nameof(TickType.AskSize),
            TickType.LastPrice => nameof(TickType.LastPrice),
            TickType.LastSize => nameof(TickType.LastSize),
            TickType.HighPrice => nameof(TickType.HighPrice),
            TickType.LowPrice => nameof(TickType.LowPrice),
            TickType.Volume => nameof(TickType.Volume),
            TickType.ClosePrice => nameof(TickType.ClosePrice),
            TickType.BidOptionComputation => nameof(TickType.BidOptionComputation),
            TickType.AskOptionComputation => nameof(TickType.AskOptionComputation),
            TickType.LastOptionComputation => nameof(TickType.LastOptionComputation),
            TickType.ModelOptionComputation => nameof(TickType.ModelOptionComputation),
            TickType.OpenPrice => nameof(TickType.OpenPrice),
            TickType.Low13Week => nameof(TickType.Low13Week),
            TickType.High13Week => nameof(TickType.High13Week),
            TickType.Low26Week => nameof(TickType.Low26Week),
            TickType.High26Week => nameof(TickType.High26Week),
            TickType.Low52Week => nameof(TickType.Low52Week),
            TickType.High52Week => nameof(TickType.High52Week),
            TickType.AverageVolume => nameof(TickType.AverageVolume),
            TickType.OpenInterest => nameof(TickType.OpenInterest),
            TickType.OptionHistoricalVolatility => nameof(TickType.OptionHistoricalVolatility),
            TickType.OptionImpliedVolatility => nameof(TickType.OptionImpliedVolatility),
            TickType.OptionBidExchange => nameof(TickType.OptionBidExchange),
            TickType.OptionAskExchange => nameof(TickType.OptionAskExchange),
            TickType.OptionCallOpenInterest => nameof(TickType.OptionCallOpenInterest),
            TickType.OptionPutOpenInterest => nameof(TickType.OptionPutOpenInterest),
            TickType.OptionCallVolume => nameof(TickType.OptionCallVolume),
            TickType.OptionPutVolume => nameof(TickType.OptionPutVolume),
            TickType.IndexFuturePremium => nameof(TickType.IndexFuturePremium),
            TickType.BidExchange => nameof(TickType.BidExchange),
            TickType.AskExchange => nameof(TickType.AskExchange),
            TickType.AuctionVolume => nameof(TickType.AuctionVolume),
            TickType.AuctionPrice => nameof(TickType.AuctionPrice),
            TickType.AuctionImbalance => nameof(TickType.AuctionImbalance),
            TickType.MarkPrice => nameof(TickType.MarkPrice),
            TickType.BidExchangeForPhysicalComputation => nameof(TickType.BidExchangeForPhysicalComputation),
            TickType.AskExchangeForPhysicalComputation => nameof(TickType.AskExchangeForPhysicalComputation),
            TickType.LastExchangeForPhysicalComputation => nameof(TickType.LastExchangeForPhysicalComputation),
            TickType.OpenExchangeForPhysicalComputation => nameof(TickType.OpenExchangeForPhysicalComputation),
            TickType.HighExchangeForPhysicalComputation => nameof(TickType.HighExchangeForPhysicalComputation),
            TickType.LowExchangeForPhysicalComputation => nameof(TickType.LowExchangeForPhysicalComputation),
            TickType.CloseExchangeForPhysicalComputation => nameof(TickType.CloseExchangeForPhysicalComputation),
            TickType.LastTimeStamp => nameof(TickType.LastTimeStamp),
            TickType.Shortable => nameof(TickType.Shortable),
            TickType.FundamentalRatios => nameof(TickType.FundamentalRatios),
            TickType.RealtimeVolume => nameof(TickType.RealtimeVolume),
            TickType.Halted => nameof(TickType.Halted),
            TickType.BidYield => nameof(TickType.BidYield),
            TickType.AskYield => nameof(TickType.AskYield),
            TickType.LastYield => nameof(TickType.LastYield),
            TickType.CustomOptionComputation => nameof(TickType.CustomOptionComputation),
            TickType.TradeCount => nameof(TickType.TradeCount),
            TickType.TradeRate => nameof(TickType.TradeRate),
            TickType.VolumeRate => nameof(TickType.VolumeRate),
            TickType.LastRegularTradingHoursTrade => nameof(TickType.LastRegularTradingHoursTrade),
            TickType.RealtimeHistoricalVolatility => nameof(TickType.RealtimeHistoricalVolatility),
            TickType.IbDividends => nameof(TickType.IbDividends),
            TickType.BondFactorMultiplier => nameof(TickType.BondFactorMultiplier),
            TickType.RegulatoryImbalance => nameof(TickType.RegulatoryImbalance),
            TickType.NewsTick => nameof(TickType.NewsTick),
            TickType.ShortTermVolume3Min => nameof(TickType.ShortTermVolume3Min),
            TickType.ShortTermVolume5Min => nameof(TickType.ShortTermVolume5Min),
            TickType.ShortTermVolume10Min => nameof(TickType.ShortTermVolume10Min),
            TickType.DelayedBidPrice => nameof(TickType.DelayedBidPrice),
            TickType.DelayedAskPrice => nameof(TickType.DelayedAskPrice),
            TickType.DelayedLastPrice => nameof(TickType.DelayedLastPrice),
            TickType.DelayedBidSize => nameof(TickType.DelayedBidSize),
            TickType.DelayedAskSize => nameof(TickType.DelayedAskSize),
            TickType.DelayedLastSize => nameof(TickType.DelayedLastSize),
            TickType.DelayedHighPrice => nameof(TickType.DelayedHighPrice),
            TickType.DelayedLowPrice => nameof(TickType.DelayedLowPrice),
            TickType.DelayedVolume => nameof(TickType.DelayedVolume),
            TickType.DelayedClosePrice => nameof(TickType.DelayedClosePrice),
            TickType.DelayedOpenPrice => nameof(TickType.DelayedOpenPrice),
            TickType.RtTrdVolume => nameof(TickType.RtTrdVolume),
            TickType.CreditmanMarkPrice => nameof(TickType.CreditmanMarkPrice),
            TickType.CreditmanSlowMarkPrice => nameof(TickType.CreditmanSlowMarkPrice),
            TickType.DelayedBidOptionComputation => nameof(TickType.DelayedBidOptionComputation),
            TickType.DelayedAskOptionComputation => nameof(TickType.DelayedAskOptionComputation),
            TickType.DelayedLastOptionComputation => nameof(TickType.DelayedLastOptionComputation),
            TickType.DelayedModelOptionComputation => nameof(TickType.DelayedModelOptionComputation),
            TickType.LastExchange => nameof(TickType.LastExchange),
            TickType.LastRegTime => nameof(TickType.LastRegTime),
            TickType.FuturesOpenInterest => nameof(TickType.FuturesOpenInterest),
            TickType.AvgOptVolume => nameof(TickType.AvgOptVolume),
            TickType.DelayedLastTimeStamp => nameof(TickType.DelayedLastTimeStamp),
            TickType.ShortableShares => nameof(TickType.ShortableShares),
            TickType.DelayedHalted => nameof(TickType.DelayedHalted),
            TickType.Reuters2MutualFunds => nameof(TickType.Reuters2MutualFunds),
            TickType.EtfNavClose => nameof(TickType.EtfNavClose),
            TickType.EtfNavPriorClose => nameof(TickType.EtfNavPriorClose),
            TickType.EtfNavBid => nameof(TickType.EtfNavBid),
            TickType.EtfNavAsk => nameof(TickType.EtfNavAsk),
            TickType.EtfNavLast => nameof(TickType.EtfNavLast),
            TickType.EtfFrozenNavLast => nameof(TickType.EtfFrozenNavLast),
            TickType.EtfNavHigh => nameof(TickType.EtfNavHigh),
            TickType.EtfNavLow => nameof(TickType.EtfNavLow),
            TickType.Undefined => nameof(TickType.Undefined),
            TickType.MarketDataType => nameof(TickType.MarketDataType),
            _ => throw new InvalidCastException(nameof(TickType))
        };
    }
}
