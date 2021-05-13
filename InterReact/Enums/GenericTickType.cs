namespace InterReact
{
    public enum GenericTickType
    {
        /// <summary>
        /// Option Volume. For stocks options only.
        /// </summary>
        OptionVolume = 100,

        /// <summary>
        /// Option Open Interest. For stocks only.
        /// </summary>
        OptionOpenInterest = 101,

        /// <summary>
        /// Historical Volatility. For stocks only.
        /// </summary>
        HistoricalVolatility = 104,

        /// <summary>
        /// Average Option Volume. For stocks only.
        /// </summary>
        AverageOptionVolume = 105,

        /// <summary>
        /// Option Implied Volatility. For stocks only.
        /// </summary>
        OptionImpliedVolatility = 106,

        CloseImpliedVolatility = 107,

        BondAnalytics = 125,

        /// <summary>
        /// Index Future Premium
        /// Returns TickType.IndexFuturePremium.
        /// </summary>
        IndexFuturePremium = 162,

        /// <summary>
        /// Miscellaneous Statistics
        /// </summary>
        MiscellaneousStatistics = 165,

        Cscreen = 166,

        /// <summary>
        /// Mark Price. Used in P/L calculations.
        /// </summary>
        MarkPrice = 221,

        /// <summary>
        /// Auction Price.
        /// </summary>
        AuctionValues = 225,

        /// <summary>
        /// Returns RealTimeVolume message which contain last trade price, size, time, total volume, VWAP, and single trade flag.
        /// </summary>
        RealtimeVolume = 233,

        Shortable = 236,

        Inventory = 256,

        Fundamentals = 258,

        CloseImpliedVolatility2 = 291,

        News = 292,

        TradeCount = 293,

        TradeRate = 294,

        VolumeRate = 295,

        LastRegularTradingHoursTrade = 318,

        ParticipationMonitor = 370,

        CttTickTag = 377,

        IbRate = 381,

        RfqTickRespTag = 384,

        Dmm = 387,

        IssuerFundamentals = 388,

        IbWarrantImpVolCompeteTick = 391,

        FuturesMargins = 407,

        RealtimeHistoricalVolatility = 411,

        MonetaryClosePrice = 428,

        MonitorTickTag = 439,

        Dividends = 456,

        /// <summary>
        /// The the closing price of the current day.
        /// </summary>
        TodayClose = 459,

        BondFactorMultiplier = 460
    }

}
