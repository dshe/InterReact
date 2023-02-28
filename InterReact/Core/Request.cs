using NodaTime.Text;

namespace InterReact;

/// <summary>
/// Methods which send request messages to TWS/Gateway.
/// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
/// </summary>
public sealed class Request
{
    private readonly Connection Connection;
    private readonly Func<RequestMessage> CreateMessage;

    public Request(Connection connection, Func<RequestMessage> requestMessageFactory)
    {
        Connection = connection;
        CreateMessage = requestMessageFactory;
    }

    /// <summary>
    /// Returns successive ids to uniquely identify requests and orders.
    /// The initial value is set during connection and may be greater than 0 in case there are previous orders.
    /// </summary>
    public int GetNextId() => Interlocked.Increment(ref Connection.Id);

    /// For testing with mock server.
    internal void RequestControl(string message) => CreateMessage()
        .Write(RequestCode.Control, message)
        .Send();

    /// <summary>
    /// Call this method to request market data, streaming or snapshot.
    /// Returns market data for an instrument either in real time or 10-15 minutes delayed (depending on the market data type specified)
    /// Include the generic Tick RealtimeVolume to receive TickRealTimeVolume ticks.
    /// </summary>
    public void RequestMarketData(
        int requestId, 
        Contract contract, 
        IEnumerable<GenericTickType>? genericTickTypes = null,
        bool isSnapshot = false, 
        bool isRegulatorySnapshot = false, 
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        RequestMessage m = CreateMessage()

        .Write(RequestCode.RequestMarketData, "11", requestId)
        .WriteContract(contract);

        if (contract.SecurityType == SecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange);
        }

        DeltaNeutralContract? dnc = contract.DeltaNeutralContract;
        m.Write(dnc is not null);
        if (dnc is not null)
            m.Write(dnc.ContractId, dnc.Delta, dnc.Price);

        m.WriteEnumValuesString(genericTickTypes)
            .Write(
                isSnapshot,
                isRegulatorySnapshot,
                Tag.Combine(options))
            .Send();
    }

    public void CancelMarketData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelMarketData, "1", requestId)
        .Send();

    public void PlaceOrder(int orderId, Order order, Contract contract) // the monster
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(contract);

        RequestMessage m = CreateMessage()
            .Write(RequestCode.PlaceOrder, orderId)
            .WriteContract(contract)
            .Write(
                contract.SecurityIdType, 
                contract.SecurityId)
            .Write(
                order.OrderAction,
                order.TotalQuantity,
                order.OrderType,
                order.LimitPrice.ToMax(),
                order.AuxPrice.ToMax(),
                order.TimeInForce, 
                order.OcaGroup,
                order.Account, 
                order.OpenClose, 
                order.Origin,
                order.OrderRef, 
                order.Transmit, 
                order.ParentId,
                order.BlockOrder, 
                order.SweepToFill, 
                order.DisplaySize,
                order.TriggerMethod, 
                order.OutsideRegularTradingHours, 
                order.Hidden);

        if (contract.SecurityType == SecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(
                    leg.ContractId, 
                    leg.Ratio, 
                    leg.TradeAction, 
                    leg.Exchange,
                    leg.OpenClose, 
                    leg.ComboShortSaleSlot, 
                    leg.DesignatedLocation, 
                    leg.ExemptCode);

            m.Write(order.OrderComboLegs.Count);
            foreach (OrderComboLeg leg in order.OrderComboLegs)
                m.Write(leg.Price.ToMax());

            m.Write(order.SmartComboRoutingParams.Count);
            foreach (Tag tag in order.SmartComboRoutingParams)
                m.Write(tag.Name, tag.Value);
        }

        m.Write(
            "", // deprecated SharesAllocation field
            order.DiscretionaryAmount, 
            order.GoodAfterTime, 
            order.GoodUntilDate,
            order.FinancialAdvisorGroup, 
            order.FinancialAdvisorMethod,
            order.FinancialAdvisorPercentage, 
            order.FinancialAdvisorProfile,
            order.ModelCode,
            order.ShortSaleSlot, 
            order.DesignatedLocation, 
            order.ExemptCode,
            order.OcaType, 
            order.Rule80A,
            order.SettlingFirm, 
            order.AllOrNone,
            order.MinQty.ToMax(), 
            order.PercentOffset.ToMax(),
            false, 
            false, 
            double.MaxValue.ToMax(),
            order.AuctionStrategy, 
            order.StartingPrice.ToMax(), 
            order.StockReferencePrice.ToMax(),
            order.Delta.ToMax(),
            order.StockRangeLower.ToMax(), 
            order.StockRangeUpper.ToMax(),
            order.OverridePercentageConstraints, 
            order.Volatility.ToMax(), 
            order.VolatilityType,
            order.DeltaNeutralOrderType, 
            order.DeltaNeutralAuxPrice.ToMax());

        if (!string.IsNullOrEmpty(order.DeltaNeutralOrderType))
        {
            m.Write(
                order.DeltaNeutralContractId, 
                order.DeltaNeutralSettlingFirm,
                order.DeltaNeutralClearingAccount, 
                order.DeltaNeutralClearingIntent,
                order.DeltaNeutralOpenClose, 
                order.DeltaNeutralShortSale, 
                order.DeltaNeutralShortSaleSlot,
                order.DeltaNeutralDesignatedLocation);
        }

        m.Write(
            order.ContinuousUpdate, 
            order.ReferencePriceType,
            order.TrailingStopPrice.ToMax(), 
            order.TrailingStopPercent.ToMax(),
            order.ScaleInitLevelSize.ToMax(), 
            order.ScaleSubsLevelSize.ToMax(), 
            order.ScalePriceIncrement.ToMax());

        if (order.ScalePriceIncrement > 0 && order.ScalePriceIncrement != double.MaxValue)
        {
            m.Write(
                order.ScalePriceAdjustValue.ToMax(), 
                order.ScalePriceAdjustInterval.ToMax(),
                order.ScaleProfitOffset.ToMax(), 
                order.ScaleAutoReset, 
                order.ScaleInitPosition.ToMax(),
                order.ScaleInitFillQty.ToMax(), 
                order.ScaleRandomPercent);
        }

        m.Write(
            order.ScaleTable,
            order.ActiveStartTime,
            order.ActiveStopTime);

        m.Write(order.HedgeType);
        if (order.HedgeType != HedgeType.Undefined)
            m.Write(order.HedgeParam);

        m.Write(
            order.OptOutSmartRouting,
            order.ClearingAccount,
            order.ClearingIntent,
            order.NotHeld);

        m.Write(contract.DeltaNeutralContract is not null);
        if (contract.DeltaNeutralContract is not null)
        {
            m.Write(
                contract.DeltaNeutralContract.ContractId, 
                contract.DeltaNeutralContract.Delta,
                contract.DeltaNeutralContract.Price);
        }

        m.Write(order.AlgoStrategy);
        if (!string.IsNullOrEmpty(order.AlgoStrategy))
        {
            m.Write(order.AlgoParams.Count);
            foreach (Tag tag in order.AlgoParams)
                m.Write(tag.Name, tag.Value);
        }

        m.Write(
            order.AlgoId, 
            order.WhatIf, 
            Tag.Combine(order.OrderMiscOptions),
            order.Solicited, 
            order.RandomizeSize, 
            order.RandomizePrice);

        if (order.OrderType == OrderType.PeggedToBenchmark)
        {
            m.Write(
                order.ReferenceContractId, 
                order.IsPeggedChangeAmountDecrease,
                order.PeggedChangeAmount, 
                order.ReferenceChangeAmount, 
                order.ReferenceExchange);
        }

        m.Write(order.Conditions.Count);
        if (order.Conditions.Count > 0)
        {
            foreach (OrderCondition condition in order.Conditions)
            {
                m.Write((int)condition.Type);
                condition.Serialize(m);
            }
            m.Write(
                order.ConditionsIgnoreRegularTradingHours, 
                order.ConditionsCancelOrder);
        }

        m.Write(
            order.AdjustedOrderType, 
            order.TriggerPrice, 
            order.LmtPriceOffset, 
            order.AdjustedStopPrice,
            order.AdjustedStopLimitPrice, 
            order.AdjustedTrailingAmount, 
            order.AdjustableTrailingUnit,
            order.ExtOperator,
            order.SoftDollarTier.Name, 
            order.SoftDollarTier.Value,
            order.CashQty.ToMax(),
            order.Mifid2DecisionMaker, 
            order.Mifid2DecisionAlgo,
            order.Mifid2ExecutionTrader, 
            order.Mifid2ExecutionAlgo,
            order.DontUseAutoPriceForHedge,
            order.IsOmsContainer,
            order.DiscretionaryUpToLimitPrice,
            order.UsePriceMgmtAlgo,
            order.Duration,
            order.PostToAts,
            order.AutoCancelParent,
            order.AdvancedErrorOverride,
            order.ManualOrderTime);

        if (contract.Exchange == "IBKRATS")
            m.Write(order.MinTradeQty.ToMax());
        
        bool sendMidOffsets = false;
        if (order.OrderType == OrderType.PeggedToBest)
        {
            m.Write(
                order.MinCompeteSize.ToMax(), 
                order.CompeteAgainstBestOffset.ToMax());
            if (order.CompeteAgainstBestOffset == double.PositiveInfinity)
                sendMidOffsets = true;
        }
        else if (order.OrderType == OrderType.PeggedToMidpoint)
            sendMidOffsets = true;
        if (sendMidOffsets)
            m.Write(
                order.MidOffsetAtWhole.ToMax(), 
                order.MidOffsetAtHalf.ToMax());
        m.Send();
    }

    public void CancelOrder(int orderId, string manualOrderCancelTime = "") => CreateMessage()
        .Write(RequestCode.CancelOrder, "1")
        .Write(orderId, manualOrderCancelTime)
        .Send();

    public void RequestOpenOrders() => CreateMessage()
        .Write(RequestCode.RequestOpenOrders, "1")
        .Send();

    /// <summary>
    /// Call this function to start receiving account updates.
    /// Returns AccountValue, PortfolioUpdate and TimeUpdate messages.
    /// Updates for all accounts are returned when accountCode is null(?).
    /// This information is updated every three minutes.
    /// </summary>
    public void RequestAccountUpdates(bool subscribe, string accountCode = "") => CreateMessage()
        .Write(RequestCode.RequestAccountData, "2")
        .Write(subscribe, accountCode)
        .Send();

    /// <summary>
    /// When this method is called, execution reports from the last 24 hours that meet the filter criteria are retrieved.
    /// To view executions beyond the past 24 hours, open the trade log in TWS and, while the Trade Log is displayed, select the days to be returned.
    /// When ExecutionFilter is null, all executions are returned.
    /// Note that the valid format for time is "yyyymmdd-hh:mm:ss"
    /// </summary>
    public void RequestExecutions(int requestId, ExecutionFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        CreateMessage()
            .Write(RequestCode.RequestExecutions, "3", requestId)
            .Write(
                filter.ClientId,
                filter.Account,
                filter.Time,
                filter.Symbol,
                filter.SecurityType,
                filter.Exchange,
                filter.Side)
            .Send();
    }

    /// <summary>
    /// Call this function to request the next available order Id.
    /// (the numids parameter has been deprecated)
    /// </summary>
    public void RequestNextOrderId() => CreateMessage()
        .Write(RequestCode.RequestIds, "1")
        .Write(-1) // numIds
        .Send();

    /// <summary>
    /// Call this method to retrieve one or more ContractDetails objects for the specified selector contract.
    /// </summary>
    public void RequestContractDetails(int requestId, Contract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.RequestContractData, "8", requestId)
            .WriteContract(contract)
            .Write(
                contract.IncludeExpired,
                contract.SecurityIdType, 
                contract.SecurityId,
                contract.IssuerId)
            .Send();
    }

    /// <summary>
    /// Call this method to request market depth for the specified contract. 
    /// </summary>
    public void RequestMarketDepth(int requestId, Contract contract, int numRows = 3, bool isSmartDepth = false, params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.RequestMarketDepth, "5", requestId)
            .WriteContract(contract)
            .Write(numRows, isSmartDepth, Tag.Combine(options))
            .Send();
    }

    public void CancelMarketDepth(int requestId, bool isSmartDepth = false) => CreateMessage()
        .Write(RequestCode.CancelMarketDepth, "1", requestId)
        .Write(isSmartDepth)
        .Send();

    /// <summary>
    /// Call this method to start receiving news bulletins, such as information about exchange status.
    /// </summary>
    public void RequestNewsBulletins(bool all = true) => CreateMessage()
        .Write(RequestCode.RequestNewsBulletins, "1")
        .Write(all)
        .Send();

    public void CancelNewsBulletins() => CreateMessage()
        .Write(RequestCode.CancelNewsBulletin, "1")
        .Send();

    public void ChangeServerLogLevel(LogEntryLevel logLevel) => CreateMessage()
        .Write(RequestCode.ChangeServerLog, "1")
        .Write(logLevel)
        .Send();

    public void RequestAutoOpenOrders(bool autoBind) => CreateMessage()
        .Write(RequestCode.RequestAutoOpenOrders, "1")
        .Write(autoBind)
        .Send();

    public void RequestAllOpenOrders() => CreateMessage()
        .Write(RequestCode.RequestAllOpenOrders, "1")
        .Send();

    public void RequestManagedAccounts() => CreateMessage()
        .Write(RequestCode.RequestManagedAccounts, "1")
        .Send();

    public void RequestFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType) => CreateMessage()
        .Write(RequestCode.RequestFA, "1")
        .Write(dataType)
        .Send();

    public void ReplaceFinancialAdvisorConfiguration(int requestId, FinancialAdvisorDataType dataType, string xml) => CreateMessage()
        .Write(RequestCode.ReplaceFA, "1")
        .Write(dataType, xml)
        .Write(requestId)
        .Send();

    private static readonly InstantPattern requestHistoricalDataDatePattern = InstantPattern.CreateWithInvariantCulture("yyyyMMdd HH:mm:ss");
    /// <summary>
    /// Call this method to receive historical data.
    /// When the "end" argument is null, data up until the current time is returned.
    /// Historical data requests are limited to 60 every 10 minutes.
    /// Errors may still occur when six or more historical data requests for the same Contract, Exchange and Tick Type are made within two seconds.
    /// Note that formatData parameter affects intra-day bars only. 1-day bars always return with date in YYYYMMDD format.
    /// Use Utility.Time.HistoricalTimespan to convert timespans to strings.
    /// </summary>
    public void RequestHistoricalData(
        int requestId,
        Contract contract,
        Instant endDate = default,
        HistoricalDuration? duration = null,
        HistoricalBarSize? barSize = null,
        HistoricalDataType? dataType = null,
        bool regularTradingHoursOnly = false,
        bool keepUpToDate = false,
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        if (endDate == default && !keepUpToDate)
            endDate = Connection.Clock.GetCurrentInstant();
        string endDateStr = requestHistoricalDataDatePattern.Format(endDate) + " GMT";
        barSize ??= HistoricalBarSize.OneHour;
        duration ??= HistoricalDuration.OneDay;
        dataType ??= HistoricalDataType.Trades;

        RequestMessage m = CreateMessage()
            .Write(RequestCode.RequestHistoricalData, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(
                endDateStr,
                barSize,
                duration,
                regularTradingHoursOnly,
                dataType,
                "1"); // return date as text

        if (contract.SecurityType == SecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange);
        }

        m.Write(keepUpToDate, Tag.Combine(options)).Send();
    }

    public void ExerciseOptions(int requestId, Contract contract, OptionExerciseAction exerciseAction,
        int exerciseQuantity, string account, bool overrideOrder)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ExerciseOptions, "2", requestId)
            .WriteContract(contract, true)
            .Write(exerciseAction, exerciseQuantity, account, overrideOrder)
            .Send();
    }

    public void RequestScannerSubscription(int requestId, ScannerSubscription subscription, 
        string subscriptionOptions = "", string subscriptionFilterOptions = "")
    {
        ArgumentNullException.ThrowIfNull(subscription);
        CreateMessage()
            .Write(RequestCode.RequestScannerSubscription, requestId)
            .Write(
                subscription.NumberOfRows.ToMax(),
                subscription.Instrument,
                subscription.LocationCode,
                subscription.ScanCode,
                subscription.AbovePrice.ToMax(), 
                subscription.BelowPrice.ToMax(), 
                subscription.AboveVolume.ToMax(),
                subscription.MarketCapAbove.ToMax(), 
                subscription.MarketCapBelow.ToMax(),
                subscription.MoodyRatingAbove, 
                subscription.MoodyRatingBelow,
                subscription.SpRatingAbove, 
                subscription.SpRatingBelow,
                subscription.MaturityDateAbove, 
                subscription.MaturityDateBelow,
                subscription.CouponRateAbove.ToMax(),
                subscription.CouponRateBelow.ToMax(),
                subscription.ExcludeConvertible,
                subscription.AverageOptionVolumeAbove.ToMax(),
                subscription.ScannerSettingPairs,
                subscription.StockType,
                subscriptionFilterOptions,
                subscriptionOptions)
            .Send();
    }

    public void CancelScannerSubscription(int requestId) => CreateMessage()
        .Write(RequestCode.CancelScannerSubscription, "1", requestId)
        .Send();

    /// <summary>
    /// Call the this method to receive an XML document that describes the valid parameters that a scanner subscription can have.
    /// </summary>ca
    public void RequestScannerParameters() => CreateMessage()
        .Write(RequestCode.RequestScannerParameters, "1")
        .Send();

    public void CancelHistoricalData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelHistoricalData, "1", requestId)
        .Send();

    /// <summary>
    /// This is the time reported by TWS. Seconds precision.
    /// </summary>
    public void RequestCurrentTime() => CreateMessage()
        .Write(RequestCode.RequestCurrentTime, "1")
        .Send();

    /// <summary>
    /// Call this method to start receiving realtime bar data.
    /// </summary>
    public void RequestRealTimeBars(int requestId, Contract contract, RealtimeBarType? whatToShow = null,
        bool regularTradingHoursOnly = true, params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        whatToShow ??= RealtimeBarType.Trades;
        CreateMessage()
            .Write(RequestCode.RequestRealTimeBars, "3", requestId)
            .WriteContract(contract)
            .Write("5", // bar size: only 5 seconds bars are available
                whatToShow,
                regularTradingHoursOnly, 
                Tag.Combine(options))
            .Send();
    }

    public void CancelRealTimeBars(int requestId) => CreateMessage()
        .Write(RequestCode.CancelRealTimeBars, "1", requestId)
        .Send();

    /// <summary>
    /// Call this method to receive Reuters global fundamental data for stocks.
    /// There must be a subscription to Reuters Fundamental set up in Account Management before you can receive this data.
    /// IB: The method can handle conid specified in the Contract object, but not tradingClass or multiplier.
    /// </summary>
    public void RequestFundamentalData(int requestId, Contract contract, FundamentalDataReportType? reportType = null, params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        reportType ??= FundamentalDataReportType.CompanyOverview;
        CreateMessage()
            .Write(RequestCode.RequestFundamentalData, "3", requestId)
            .Write(
                contract.ContractId,
                contract.Symbol,
                contract.SecurityType,
                contract.Exchange,
                contract.PrimaryExchange,
                contract.Currency,
                contract.LocalSymbol)
            .Write(reportType.ToString(), Tag.Combine(options))
            .Send();
    }

    public void CancelFundamentalData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelFundamentalData, "1", requestId)
        .Send();

    /// <summary>
    /// Call this function to calculate volatility for a supplied option price and underlying price.
    /// </summary>
    public void CalculateImpliedVolatility(int requestId, Contract contract, double optionPrice, double underlyingPrice, params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqCalcImpliedVolat, "3", requestId)
            .WriteContract(contract)
            .Write(optionPrice, underlyingPrice, Tag.Combine(options))
            .Send();
    }

    /// <summary>
    /// Call this function to calculate option price and greek Values for a supplied volatility and underlying price.
    /// </summary>
    public void CalculateOptionPrice(int requestId, Contract contract, double volatility, double underlyingPrice, params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqCalcOptionPrice, "3", requestId)
            .WriteContract(contract)
            .Write(volatility, underlyingPrice, Tag.Combine(options))
            .Send();
    }

    public void CancelCalculateImpliedVolatility(int requestId) => CreateMessage()
        .Write(RequestCode.CancelImpliedVolatility, "1", requestId)
        .Send();

    public void CancelCalculateOptionPrice(int requestId) => CreateMessage()
        .Write(RequestCode.CancelOptionPrice, "1", requestId)
        .Send();

    /// <summary>
    /// Use this method to cancel all open orders globally. It cancels both API and TWS open orders.
    /// </summary>
    public void RequestGlobalCancel() => CreateMessage()
        .Write(RequestCode.RequestGlobalCancel, "1")
        .Send();

    /// <summary>
    /// The API can receive frozen market data from TWS. Frozen market data is the last data recorded in IB's system.
    /// During normal trading hours, the API receives real-time market data.
    /// If you use this function, you are telling TWS to automatically switch to frozen market data after the close.
    /// Then, before the opening of the next trading day, market data will automatically switch back to real-time market data.
    /// </summary>
    public void RequestMarketDataType(MarketDataType marketDataType) => CreateMessage()
        .Write(RequestCode.RequestMarketDataType, "1")
        .Write(marketDataType)
        .Send();

    /// <summary>
    /// Subscribes to position updates for all accessible accounts. All positions sent initially, and then only updates as positions change. 
    /// </summary>
    public void RequestPositions() => CreateMessage()
        .Write(RequestCode.RequestPositions, "1")
        .Send();

    /// <summary>
    /// Subscribe to data that appears on the TWS Account Window Summary tab. 
    /// If no tags are specified, data for all tags will be requested.
    /// </summary>
    public void RequestAccountSummary(int requestId, string group = "All", params AccountSummaryTag[] tags)
    {
        if (!tags.Any())
            tags = Enum.GetValues<AccountSummaryTag>().ToArray();
        List<string> tagNames = tags.Select(tag => tag.ToString()).ToList();
        CreateMessage()
            .Write(RequestCode.RequestAccountSummary, "1", requestId)
            .Write(group, string.Join(",", tagNames))
            .Send();
    }

    public void CancelAccountSummary(int requestId) => CreateMessage()
        .Write(RequestCode.CancelAccountSummary, "1", requestId)
        .Send();

    public void CancelPositions() => CreateMessage()
        .Write(RequestCode.CancelPositions, "1")
        .Send();

    // 65, 66: For IB internal use

    public void QueryDisplayGroups(int requestId) => CreateMessage()
        .Write(RequestCode.QueryDisplayGroups, "1", requestId)
        .Send();

    public void SubscribeToGroupEvents(int requestId, int groupId) => CreateMessage()
        .Write(RequestCode.SubscribeToGroupEvents, "1", requestId)
        .Write(groupId)
        .Send();

    public void UpdateDisplayGroup(int requestId, string contractInfo) => CreateMessage()
        .Write(RequestCode.UpdateDisplayGroup, "1", requestId)
        .Write(contractInfo)
        .Send();

    public void UnsubscribeFromGroupEvents(int requestId) => CreateMessage()
        .Write(RequestCode.UnsubscribeFromGroupEvents, "1", requestId)
        .Send();

    // 71: StartApi
    // 72, 73: for IB internal use

    /**
    * Requests position subscription for account and/or model
    * Initially all positions are returned, and then updates are returned for any position changes in real time.
    * If an account Id is provided, only the account's positions belonging to the specified model will be delivered
    */
    public void RequestPositionsMulti(int requestId, string account, string modelCode) => CreateMessage()
        .Write(RequestCode.RequestPositionsMulti, "1", requestId)
        .Write(account, modelCode)
        .Send();

    public void CancelPositionsMulti(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPositionsMulti, "1", requestId)
        .Send();

    public void RequestAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNlv) => 
        CreateMessage()
            .Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId)
            .Write(account, modelCode, ledgerAndNlv)
            .Send();

    public void CancelAccountUpdatesMulti(int requestId) => CreateMessage()
        .Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId)
        .Send();

    public void RequestSecDefOptParams(int requestId, string underlyingSymbol,
        string futFopExchange, string underlyingSecType, int underlyingConId) => 
            CreateMessage()
                .Write(RequestCode.RequestSecurityDefinitionOptionalParameters, requestId)
                .Write(underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId)
                .Send();
 
    public void RequestSoftDollarTiers(int requestId) => CreateMessage()
        .Write(RequestCode.RequestSoftDollarTiers, requestId)
        .Send();

    public void RequestFamilyCodes() => CreateMessage()
        .Write(RequestCode.RequestFamilyCodes)
        .Send();

    public void RequestMatchingSymbols(int requestId, string pattern) => CreateMessage()
        .Write(RequestCode.RequestMatchingSymbols, requestId)
        .Write(pattern)
        .Send();

    // Discover exchanges for which market data is returned to updateMktDepthL2(those with market makers)
    public void RequestMarketDepthExchanges() => CreateMessage()
        .Write(RequestCode.RequestMktDepthExchanges)
        .Send();

    public void RequestSmartComponents(int requestId, string bboExchange) => CreateMessage()
        .Write(RequestCode.RequestSmartComponents, requestId)
        .Write(bboExchange)
        .Send();

    public void RequestNewsArticle(int requestId, string providerCode, string articleId, params Tag[] options) => CreateMessage()
        .Write(RequestCode.RequestNewsArticle, requestId)
        .Write(providerCode, articleId)
        .Write(Tag.Combine(options))
        .Send();

    public void RequestNewsProviders() => CreateMessage()
        .Write(RequestCode.RequestNewsProviders)
        .Send();

    public void RequestHistoricalNews(int requestId, int contractId, string providerCodes,
        string startTime, string endTime, int totalResults, params Tag[] options) => 
        CreateMessage()
            .Write(RequestCode.RequestHistoricalNews, requestId)
            .Write(contractId, providerCodes, startTime, endTime, totalResults)
            .Write(Tag.Combine(options))
            .Send();

    // Returns the timestamp of earliest available historical data for a contract and data type
    // whatToShow - type of data for head timestamp - "BID", "ASK", "TRADES", etc
    // useRTH - use regular trading hours only, 1 for yes or 0 for no
    // formatDate - @param formatDate set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds
    public void RequestHeadTimestamp(int requestId, Contract contract, string whatToShow, int useRth, int formatDate)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.RequestHeadTimestamp, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(useRth, whatToShow, formatDate)
            .Send();
    }

    public void RequestHistogramData(int requestId, Contract contract, bool useRth, string period)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.RequestHistogramData, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(useRth, period)
            .Send();
    }

    public void CancelHistogramData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelHistogramData, requestId)
        .Send();

    public void CancelHeadTimestamp(int requestId) => CreateMessage()
        .Write(RequestCode.CancelHeadTimestamp, requestId)
        .Send();

    public void RequestMarketRule(int marketRuleId) => CreateMessage()
        .Write(RequestCode.RequestMarketRule, marketRuleId)
        .Send();

    public void RequestPnL(int requestId, string account, string modelCode) => CreateMessage()
        .Write(RequestCode.ReqPnL, requestId)
        .Write(account, modelCode)
        .Send();

    public void CancelPnL(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPnL, requestId)
        .Send();

    public void RequestPnLSingle(int requestId, string account, string modelCode, int contractId) => CreateMessage()
        .Write(RequestCode.ReqPnLSingle, requestId)
        .Write(account, modelCode, contractId)
        .Send();

    public void CancelPnLSingle(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPnLSingle, requestId)
        .Send();

    public void RequestHistoricalTicks(int requestId, Contract contract, string startDateTime, string endDateTime,
        int numberOfTicks, string whatToShow, int useRth, bool ignoreSize, params Tag[] miscOptions)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqHistoricalTicks, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(startDateTime, endDateTime, numberOfTicks, whatToShow, useRth, ignoreSize)
            .Write(Tag.Combine(miscOptions))
            .Send();
    }

    public void RequestTickByTickData(int requestId, Contract contract, string tickType, int numberOfTicks, bool ignoreSize)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqTickByTickData, requestId)
            .WriteContract(contract)
            .Write(tickType, numberOfTicks, ignoreSize)
            .Send();
    }

    public void CancelTickByTickData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelTickByTickData, requestId)
        .Send();

    public void RequestCompletedOrders(bool apiOnly) => CreateMessage()
        .Write(RequestCode.ReqCompletedOrders, apiOnly)
        .Send();

    public void RequestWshMetaData(int requestId) => CreateMessage()
        .Write(RequestCode.ReqWshMetaData, requestId)
        .Send();

    public void CancelWshMetaData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelWshMetaData, requestId)
        .Send();

    public void RequestWshEventData(int requestId, WshEventData wshEventData)
    {
        ArgumentNullException.ThrowIfNull(wshEventData);
        CreateMessage()
            .Write(RequestCode.ReqWshEventData, requestId)
            .Write(
                wshEventData.ContractId,
                wshEventData.Filter,
                wshEventData.FillWatchlist,
                wshEventData.FillPortfolio,
                wshEventData.FillCompetitors,
                wshEventData.StartDate,
                wshEventData.EndDate,
                wshEventData.TotalLimit)
            .Send();
    }

    public void CancelWshEventData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelWshEventData, requestId)
        .Send();

    public void RequestUserInformation(int requestId) => CreateMessage()
        .Write(RequestCode.ReqUserInfo, requestId)
        .Send();
}
