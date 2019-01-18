namespace InterReact.Enums
{
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
        DelayedHigh = 72,
        DelayedLow = 73,
        DelayedVolume = 74,
        DelayedClose = 75,
        DelayedOpen = 76,
        RtTrdVolume = 77,
        CreditmanMarkPrice = 78,
        CreditmanSlowMarkPrice = 79,
        DelayedBidOption = 80,
        DelayedAskOption = 81,
        DelayedLastOption = 82,
        DelayedModelOption = 83,
        LastExchange = 84,
        LastRegTime = 85
    }

    internal static class TickTypeSize
    {
        internal static (bool success, TickType tickType) GetTickTypeSize(TickType tickType)
        {
            switch (tickType)
            {
                case TickType.BidPrice:
                    return (true, TickType.BidSize);
                case TickType.AskPrice:
                    return (true, TickType.AskSize);
                case TickType.LastPrice:
                    return (true, TickType.LastSize);
                case TickType.DelayedBidPrice:
                    return (true, TickType.DelayedBidSize);
                case TickType.DelayedAskPrice:
                    return (true, TickType.DelayedAskSize);
                case TickType.DelayedLastPrice:
                    return (true, TickType.DelayedLastSize);
                default:
                    return (false, TickType.Undefined);
            }
        }
    }
}
