namespace InterReact;

/// <summary>
/// Methods which send request messages to TWS/Gateway.
/// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
/// </summary>
public sealed class Request
{
    private InterReactOptions Options { get; }
    private Func<RequestMessage> CreateMessage { get; }

    public Request(InterReactOptions options, Func<RequestMessage> requestMessageFactory)
    {
        Options = options;
        CreateMessage = requestMessageFactory;
    }

    /// <summary>
    /// Returns successive ids to uniquely identify requests and orders.
    /// The initial value is set during connection and may be greater than 0 in case there are previous orders.
    /// </summary>
    public int GetNextId() => Interlocked.Increment(ref Options.Id);

    /// For testing with mock server.
    internal void RequestControl(string message) => CreateMessage()
        .Write(RequestCode.Control, message)
        .Send();

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

        if (contract.SecurityType == ContractSecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.Action, leg.Exchange);
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
                order.Action,
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

        if (contract.SecurityType == ContractSecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(
                    leg.ContractId,
                    leg.Ratio,
                    leg.Action,
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
        if (order.HedgeType.Length != 0)
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

        if (order.OrderType == OrderTypes.PeggedToBenchmark)
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
        if (order.OrderType == OrderTypes.PeggedToBest)
        {
            m.Write(
                order.MinCompeteSize.ToMax(),
                order.CompeteAgainstBestOffset.ToMax());
            if (order.CompeteAgainstBestOffset == double.PositiveInfinity)
                sendMidOffsets = true;
        }
        else if (order.OrderType == OrderTypes.PeggedToMidpoint)
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
    /// In case there if more than one managed account, the account must be specified.
    /// Returns AccountValue, PortfolioUpdate and TimeUpdate messages.
    /// Updates for all accounts are returned when accountCode is empty.
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

    public void RequestMarketDepth(
        int requestId,
        Contract contract,
        int numRows = 3,
        bool isSmartDepth = false,
        params Tag[] options)
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

    public void ReplaceFinancialAdvisorConfiguration(
        int requestId,
        FinancialAdvisorDataType dataType,
        string xml) => CreateMessage()
            .Write(RequestCode.ReplaceFA, "1")
            .Write(dataType, xml)
            .Write(requestId)
            .Send();

    public void RequestHistoricalData(
        int requestId,
        Contract contract,
        string endDateTime = "",
        string duration = HistoricalDataDuration.OneMonth,
        string barSize = HistoricalDataBarSize.OneHour,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        int dateFormat = 1,  // 1: yyyyMMdd HH:mm:ss, 2: time format in seconds
        bool keepUpToDate = false,
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(endDateTime);
        if (endDateTime.Length != 0 && keepUpToDate)
            throw new InvalidOperationException("RequestHistoricalData: endDate many not be specified when keepUpToDate = true.");

        RequestMessage m = CreateMessage()
            .Write(RequestCode.RequestHistoricalData, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(
                endDateTime,
                barSize,
                duration,
                regularTradingHoursOnly,
                whatToShow,
                dateFormat);

        if (contract.SecurityType == ContractSecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.Action, leg.Exchange);
        }

        m.Write(keepUpToDate, Tag.Combine(options)).Send();
    }

    public void ExerciseOptions(
        int requestId,
        Contract contract,
        OptionExerciseAction exerciseAction,
        int exerciseQuantity,
        string account,
        bool overrideOrder)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ExerciseOptions, "2", requestId)
            .WriteContract(contract, true)
            .Write(exerciseAction, exerciseQuantity, account, overrideOrder)
            .Send();
    }

    public void RequestScannerSubscription(
        int requestId,
        ScannerSubscription subscription,
        string subscriptionOptions = "",
        string subscriptionFilterOptions = "")
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

    public void RequestScannerParameters() => CreateMessage()
        .Write(RequestCode.RequestScannerParameters, "1")
        .Send();

    public void CancelHistoricalData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelHistoricalData, "1", requestId)
        .Send();

    public void RequestCurrentTime() => CreateMessage()
        .Write(RequestCode.RequestCurrentTime, "1")
        .Send();

    public void RequestRealTimeBars(
        int requestId,
        Contract contract,
        string whatToShow = RealtimeBarWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
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

    public void RequestFundamentalData(
        int requestId,
        Contract contract,
        string reportType = FundamentalDataReportType.CompanyOverview,
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
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
            .Write(reportType, Tag.Combine(options))
            .Send();
    }

    public void CancelFundamentalData(int requestId) => CreateMessage()
        .Write(RequestCode.CancelFundamentalData, "1", requestId)
        .Send();

    public void CalculateImpliedVolatility(
        int requestId,
        Contract contract,
        double optionPrice,
        double underlyingPrice,
        params Tag[] options)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqCalcImpliedVolat, "3", requestId)
            .WriteContract(contract)
            .Write(optionPrice, underlyingPrice, Tag.Combine(options))
            .Send();
    }

    public void CalculateOptionPrice(
        int requestId,
        Contract contract,
        double volatility,
        double underlyingPrice,
        params Tag[] options)
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

    public void RequestGlobalCancel() => CreateMessage()
        .Write(RequestCode.RequestGlobalCancel, "1")
        .Send();

    public void RequestMarketDataType(MarketDataType marketDataType) => CreateMessage()
        .Write(RequestCode.RequestMarketDataType, "1")
        .Write(marketDataType)
        .Send();

    public void RequestPositions() => CreateMessage()
        .Write(RequestCode.RequestPositions, "1")
        .Send();

    public void RequestAccountSummary(
        int requestId,
        string group = "All",
        params AccountSummaryTag[] tags)
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

    public void RequestPositionsMulti(
        int requestId,
        string account,
        string modelCode) => CreateMessage()
            .Write(RequestCode.RequestPositionsMulti, "1", requestId)
            .Write(account, modelCode)
            .Send();

    public void CancelPositionsMulti(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPositionsMulti, "1", requestId)
        .Send();

    public void RequestAccountUpdatesMulti(
        int requestId,
        string account,
        string modelCode,
        bool ledgerAndNlv) => CreateMessage()
            .Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId)
            .Write(account, modelCode, ledgerAndNlv)
            .Send();

    public void CancelAccountUpdatesMulti(int requestId) => CreateMessage()
        .Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId)
        .Send();

    public void RequestSecDefOptParams(
        int requestId,
        string underlyingSymbol,
        string futFopExchange,
        string underlyingSecType,
        int underlyingConId) => CreateMessage()
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

    public void RequestMarketDepthExchanges() => CreateMessage()
        .Write(RequestCode.RequestMktDepthExchanges)
        .Send();

    public void RequestSmartComponents(int requestId, string bboExchange) => CreateMessage()
        .Write(RequestCode.RequestSmartComponents, requestId)
        .Write(bboExchange)
        .Send();

    public void RequestNewsArticle(
        int requestId,
        string providerCode,
        string articleId) => CreateMessage()
            .Write(RequestCode.RequestNewsArticle, requestId)
            .Write(providerCode, articleId)
            .Write("")
            .Send();

    public void RequestNewsProviders() => CreateMessage()
        .Write(RequestCode.RequestNewsProviders)
        .Send();

    public void RequestHistoricalNews(
        int requestId,
        int contractId,
        string providerCodes,
        string startTime,
        string endTime,
        int totalResults) => CreateMessage()
            .Write(RequestCode.RequestHistoricalNews, requestId)
            .Write(contractId, providerCodes, startTime, endTime, totalResults)
            .Write("")
            .Send();

    public void RequestHeadTimestamp(
        int requestId,
        Contract contract,
        string whatToShow,
        int useRth,
        int formatDate)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.RequestHeadTimestamp, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(useRth, whatToShow, formatDate)
            .Send();
    }

    public void RequestHistogramData(
        int requestId,
        Contract contract,
        bool useRth,
        string period)
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

    public void RequestPnL(
        int requestId,
        string account,
        string modelCode) => CreateMessage()
            .Write(RequestCode.ReqPnL, requestId)
            .Write(account, modelCode)
            .Send();

    public void CancelPnL(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPnL, requestId)
        .Send();

    public void RequestPnLSingle(
        int requestId,
        string account,
        string modelCode,
        int contractId) => CreateMessage()
            .Write(RequestCode.ReqPnLSingle, requestId)
            .Write(account, modelCode, contractId)
            .Send();

    public void CancelPnLSingle(int requestId) => CreateMessage()
        .Write(RequestCode.CancelPnLSingle, requestId)
        .Send();

    public void RequestHistoricalTicks(
        int requestId,
        Contract contract,
        string startDateTime,
        string endDateTime,
        int numberOfTicks,
        string whatToShow,
        int useRth,
        bool ignoreSize)
    {
        ArgumentNullException.ThrowIfNull(contract);
        CreateMessage()
            .Write(RequestCode.ReqHistoricalTicks, requestId)
            .WriteContract(contract)
            .Write(contract.IncludeExpired)
            .Write(startDateTime, endDateTime, numberOfTicks, whatToShow, useRth, ignoreSize)
            .Write("")
            .Send();
    }

    public void RequestTickByTickData(
        int requestId,
        Contract contract,
        string tickType,
        int numberOfTicks,
        bool ignoreSize)
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
