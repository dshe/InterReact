namespace InterReact
{
    public enum ServerVersion
    {
        VT100 = 100, // min support
        FractionalPositions = 101,
        PeggedToBenchmark = 102, // (old min support)
        ModelsSupport = 103,
        SecurityDefinitionOptionalParametersRequest = 104,
        ExtOperator = 105,
        SoftDollarTier = 106, // max support of version 9.72 (stable)
        RequestFamilyCodes = 107,
        RequestMatchingSymbols = 108,
        PastLimit = 109,
        MdSizeMultiplier = 110,
        CashQty = 111,
        RequestMktDepthExchanges = 112,
        TickNews = 113,
        SmartComponents = 114,
        RequestNewsProviders = 115,
        RequestNewsArticle = 116,
        RequestHistoricalNews = 117,
        RequestHeadTimestamp = 118,
        RequestHistogramData = 119,
        ServiceDataType = 120,
        AggGroup = 121,
        UnderlyingInfo = 122, // max support
        CancelHeadstamp = 123,
        SyntRealtimeBars = 124,
        CfdReroute = 125,
        MarketRules = 126,
        Pnl = 127,
        NewsQueryOrigins = 128,
        UnrealizedPnl = 129,
        HistoricalTicks = 130, // ???
        MarketCapPrice = 131,
        PreOpenBidAsk = 132,
        RealExpirationDate = 134,
        RealizedPnl = 135,
        LastLiqidity = 136,
        TickByTick = 137,
        DecisionMaker = 138,
        MifidExecution = 139,
        TickByTickIgnoreSize = 140,
        AutoPriceForHedge = 141,
        WhatIfExtFields = 142,
        ScannerGenericOpts = 143,
        ApiBindOrder = 144,
        OrderContainer = 145,
        SmartDepth = 146,
        RemoveNullAllCasting = 147,
        DPegOrders = 148,
        MktDepthPrimExchange = 149,
        ReqCompletedOorders = 150,
        PriceMgmtAlgo = 151, // max support of 9.76 (latest)
        STOCK_TYPE = 152,
        ENCODE_MSG_ASCII7 = 153,
        SEND_ALL_FAMILY_CODES = 154,
        NO_DEFAULT_OPEN_CLOSE = 155,
        PRICE_BASED_VOLATILITY = 156,
        REPLACE_FA_END = 157
    }
}
