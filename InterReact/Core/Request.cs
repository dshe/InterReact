using System.Globalization;
namespace InterReact;

/// <summary>
/// Methods which send request messages to TWS/Gateway.
/// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
/// </summary>
public sealed class Request(Func<RequestMessage> CreateMessage, InterReactOptions Options)
{
    /// <summary>
    /// Returns successive ids to uniquely identify requests and orders.
    /// The initial value is set during connection and may be greater than 0 in case there are previous orders.
    /// </summary>
    public int GetNextId() => Interlocked.Increment(ref Options.Id);

    public async ValueTask RequestMarketDataAsync(
        int requestId,
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isSnapshot = false,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null)
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
            .Write(isSnapshot, isRegulatorySnapshot, Tag.Combine(options));

        await m.SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelMarketDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelMarketData, "1", requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask PlaceOrderAsync(int orderId, Order order, Contract contract) // monster
    {
        if (Options.AllowOrderPlacement == false)
            throw new InvalidOperationException("To place orders, first set Options.AllowOrderPlacement to true.");

        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(contract);

        RequestMessage m = CreateMessage()
            .Write(RequestCode.PlaceOrder, orderId)
            .WriteContract(contract)
            .Write(contract.SecurityIdType, contract.SecurityId)
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
        if (order.HedgeType.Code.Length != 0)
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
        await m.SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelOrderAsync(int orderId, string manualOrderCancelTime = "") =>
        await CreateMessage().Write(RequestCode.CancelOrder, "1", orderId, manualOrderCancelTime).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestOpenOrdersAsync() => 
        await CreateMessage().Write(RequestCode.RequestOpenOrders, "1").SendAsync().ConfigureAwait(false);

    /// <summary>
    /// Call this function to start receiving account updates.
    /// In case there if more than one managed account, the account must be specified.
    /// Returns AccountValue, PortfolioUpdate and TimeUpdate messages.
    /// Updates for all accounts are returned when accountCode is empty.
    /// This information is updated every three minutes.
    /// </summary>
    public async ValueTask RequestAccountUpdatesAsync(bool subscribe, string accountCode = "") =>
        await CreateMessage().Write(RequestCode.RequestAccountData, "2", subscribe, accountCode).SendAsync().ConfigureAwait(false);

    /// <summary>
    /// When this method is called, execution reports from the last 24 hours that meet the filter criteria are retrieved.
    /// To view executions beyond the past 24 hours, open the trade log in TWS and, while the Trade Log is displayed, select the days to be returned.
    /// When ExecutionFilter is null, all executions are returned.
    /// Note that the valid format for time is "yyyymmdd-hh:mm:ss"
    /// </summary>
    public async ValueTask RequestExecutionsAsync(int requestId, ExecutionFilter? filter = null)
    {
        RequestMessage m = CreateMessage()
            .Write(RequestCode.RequestExecutions, "3", requestId);
        if (filter is not null)
        {
            m.Write(
                filter.ClientId,
                filter.Account,
                filter.Time,
                filter.Symbol,
                filter.SecurityType,
                filter.Exchange,
                filter.Side);
        }
        await m.SendAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Call this function to request the next available Id.
    /// (the numids parameter has been deprecated)
    /// </summary>
    public async ValueTask RequestNextOrderIdAsync() => await CreateMessage().Write(RequestCode.RequestIds, "1", 1).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestContractDetailsAsync(int requestId, Contract contract, bool includeExpired = false)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.RequestContractData, "8", requestId)
            .WriteContract(contract)
            .Write(
                includeExpired,
                contract.SecurityIdType,
                contract.SecurityId,
                contract.IssuerId)
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask RequestMarketDepthAsync(
        int requestId,
        Contract contract,
        int numRows = 3,
        bool isSmartDepth = false,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.RequestMarketDepth, "5", requestId)
            .WriteContract(contract)
            .Write(numRows, isSmartDepth, Tag.Combine(options))
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelMarketDepthAsync(int requestId, bool isSmartDepth = false) => 
        await CreateMessage().Write(RequestCode.CancelMarketDepth, "1", requestId, isSmartDepth).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestNewsBulletinsAsync(bool all = true) => 
        await CreateMessage().Write(RequestCode.RequestNewsBulletins, "1", all).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelNewsBulletinsAsync() => 
        await CreateMessage().Write(RequestCode.CancelNewsBulletin, "1").SendAsync().ConfigureAwait(false);
    public async ValueTask SetServerLogLevelAsync(LogEntryLevel logLevel) => 
        await CreateMessage().Write(RequestCode.ChangeServerLog, "1", logLevel).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestAutoOpenOrdersAsync(bool autoBind) => 
        await CreateMessage().Write(RequestCode.RequestAutoOpenOrders, "1", autoBind).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestAllOpenOrdersAsync() => 
        await CreateMessage().Write(RequestCode.RequestAllOpenOrders, "1").SendAsync().ConfigureAwait(false);
    public async ValueTask RequestManagedAccountsAsync() => 
        await CreateMessage().Write(RequestCode.RequestManagedAccounts, "1").SendAsync().ConfigureAwait(false);
    public async ValueTask RequestFinancialAdvisorConfigurationAsync(FinancialAdvisorDataType dataType) => 
        await CreateMessage().Write(RequestCode.RequestFA, "1", dataType).SendAsync().ConfigureAwait(false);
    public async ValueTask ReplaceFinancialAdvisorConfigurationAsync(int requestId, FinancialAdvisorDataType dataType, string xml) =>
        await CreateMessage().Write(RequestCode.ReplaceFA, "1", dataType, xml, requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestHistoricalDataAsync(
        int requestId,
        Contract contract,
        string endDateTime = "",
        string duration = HistoricalDataDuration.OneMonth,
        string barSize = HistoricalDataBarSize.OneHour,
        string whatToShow = HistoricalDataWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        int dateFormat = 1,  // 1: yyyyMMdd HH:mm:ss, 2: time format in seconds
        bool keepUpToDate = false,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(endDateTime);
        if (endDateTime.Length != 0 && keepUpToDate)
            throw new InvalidOperationException("RequestHistoricalData: endDate many not be specified when keepUpToDate = true.");

        RequestMessage m = CreateMessage()
            .Write(RequestCode.RequestHistoricalData, requestId)
            .WriteContract(contract)
            .Write(true) // include expired
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
        m.Write(keepUpToDate, Tag.Combine(options));

        await m.SendAsync().ConfigureAwait(false);
    }

    public async ValueTask ExerciseOptionsAsync(
        int requestId,
        Contract contract,
        OptionExerciseAction exerciseAction,
        int exerciseQuantity,
        string account,
        bool overrideOrder)
        //string manualOrderTime
        //string customerAccount,
        //bool professionalCustomer)
    {
        ArgumentNullException.ThrowIfNull(contract);
        RequestMessage m = CreateMessage()
            .Write(RequestCode.ExerciseOptions, "2", requestId)
            .WriteContract(contract, includePrimaryExchange: false)
            .Write(exerciseAction, exerciseQuantity, account, overrideOrder);
        //if (Options.ServerVersionCurrent >= ServerVersion.MANUAL_ORDER_TIME_EXERCISE_OPTIONS)
        //    m.Write(manualOrderTime);
        //if (Options.ServerVersionCurrent >= ServerVersion.CUSTOMER_ACCOUNT) 
        //    m.Write(customerAccount);
        //if (Options.ServerVersionCurrent >= ServerVersion.PROFESSIONAL_CUSTOMER)
        //    m.Write(professionalCustomer);
        await m.SendAsync().ConfigureAwait(false);
    }

    public async ValueTask RequestScannerSubscriptionAsync(
        int requestId,
        ScannerSubscription subscription,
        string subscriptionOptions = "",
        string subscriptionFilterOptions = "")
    {
        ArgumentNullException.ThrowIfNull(subscription);
        await CreateMessage()
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
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelScannerSubscriptionAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelScannerSubscription, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestScannerParametersAsync() => 
        await CreateMessage().Write(RequestCode.RequestScannerParameters, "1").SendAsync().ConfigureAwait(false);
    public async ValueTask CancelHistoricalDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelHistoricalData, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestCurrentTimeAsync() =>
        await CreateMessage().Write(RequestCode.RequestCurrentTime, "1").SendAsync().ConfigureAwait(false);

    public async ValueTask RequestRealTimeBarsAsync(
        int requestId,
        Contract contract,
        int barSize, // this parameter is not currently used
        string whatToShow = RealtimeBarWhatToShow.Trades,
        bool regularTradingHoursOnly = true,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.RequestRealTimeBars, "3", requestId)
            .WriteContract(contract)
            .Write(barSize,
                whatToShow,
                regularTradingHoursOnly,
                Tag.Combine(options))
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelRealTimeBarsAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelRealTimeBars, "1", requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestFundamentalDataAsync(
        int requestId,
        Contract contract,
        string reportType = FundamentalDataReportType.CompanyOverview,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
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
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelFundamentalDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelFundamentalData, "1", requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask CalculateImpliedVolatilityAsync(
        int requestId,
        Contract contract,
        double optionPrice,
        double underlyingPrice,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.ReqCalcImpliedVolat, "3", requestId)
            .WriteContract(contract)
            .Write(optionPrice, underlyingPrice, Tag.Combine(options))
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CalculateOptionPriceAsync(
        int requestId,
        Contract contract,
        double volatility,
        double underlyingPrice,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.ReqCalcOptionPrice, "3", requestId)
            .WriteContract(contract)
            .Write(volatility, underlyingPrice, Tag.Combine(options))
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelCalculateImpliedVolatilityAsync(int requestId) =>
        await CreateMessage().Write(RequestCode.CancelImpliedVolatility, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelCalculateOptionPriceAsync(int requestId) =>
        await CreateMessage().Write(RequestCode.CancelOptionPrice, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestGlobalCancelAsync() =>
        await CreateMessage().Write(RequestCode.RequestGlobalCancel, "1").SendAsync().ConfigureAwait(false);
    public async ValueTask RequestMarketDataTypeAsync(MarketDataType marketDataType) =>
        await CreateMessage().Write(RequestCode.RequestMarketDataType, "1", marketDataType).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestPositionsAsync() =>
        await CreateMessage().Write(RequestCode.RequestPositions, "1").SendAsync().ConfigureAwait(false);

    public async ValueTask RequestAccountSummaryAsync(
        int requestId,
        string group = "All",
        IList<AccountSummaryTag>? tags = null)
    {
        if (tags is null || tags.Count == 0)
            tags = [.. Enum.GetValues<AccountSummaryTag>()];
        List<string> tagNames = tags.Select(tag => tag.ToString()).ToList();
        tagNames.Add("$LEDGER:ALL");
        await CreateMessage()
            .Write(RequestCode.RequestAccountSummary, "1", requestId)
            .Write(group, string.Join(",", tagNames))
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelAccountSummaryAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelAccountSummary, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelPositionsAsync() => 
        await CreateMessage().Write(RequestCode.CancelPositions, "1").SendAsync().ConfigureAwait(false);

    // 65, 66: For IB internal use.

    public async ValueTask QueryDisplayGroupsAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.QueryDisplayGroups, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask SubscribeToGroupEventsAsync(int requestId, int groupId) =>
        await CreateMessage().Write(RequestCode.SubscribeToGroupEvents, "1", requestId, groupId).SendAsync().ConfigureAwait(false);
    public async ValueTask UpdateDisplayGroupAsync(int requestId, string contractInfo) => 
        await CreateMessage().Write(RequestCode.UpdateDisplayGroup, "1", requestId, contractInfo).SendAsync().ConfigureAwait(false);
    public async ValueTask UnsubscribeFromGroupEventsAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.UnsubscribeFromGroupEvents, "1", requestId).SendAsync().ConfigureAwait(false);

    // 71: StartApi.
    // 72, 73: for IB internal use.

    // Note that RequestPositionsMulti and RequestAccountUpdatesMulti require an account code when there are multiple accounts.
    public async ValueTask RequestPositionsMultiAsync(int requestId, string account, string modelCode = "") => 
        await CreateMessage().Write(RequestCode.RequestPositionsMulti, "1", requestId, account, modelCode).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelPositionsMultiAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelPositionsMulti, "1", requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestAccountUpdatesMultiAsync(int requestId, string account, string modelCode = "", bool ledgerAndNlv = false) =>
        await CreateMessage().Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId, account, modelCode, ledgerAndNlv).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelAccountUpdatesMultiAsync(int requestId) =>
        await CreateMessage().Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestSecDefOptParamsAsync(
        int requestId,
        string underlyingSymbol,
        string futFopExchange,
        string underlyingSecType,
        int underlyingConId) 
            => await CreateMessage()
                .Write(RequestCode.RequestSecurityDefinitionOptionalParameters, requestId)
                .Write(underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId)
                .SendAsync().ConfigureAwait(false);

    public async ValueTask RequestSoftDollarTiersAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.RequestSoftDollarTiers, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestFamilyCodesAsync() =>
        await CreateMessage().Write(RequestCode.RequestFamilyCodes).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestMatchingSymbolsAsync(int requestId, string pattern) => 
        await CreateMessage().Write(RequestCode.RequestMatchingSymbols, requestId, pattern).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestMarketDepthExchangesAsync() => 
        await CreateMessage().Write(RequestCode.RequestMktDepthExchanges).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestSmartComponentsAsync(int requestId, string bboExchange) => 
        await CreateMessage().Write(RequestCode.RequestSmartComponents, requestId, bboExchange).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestNewsArticleAsync(
        int requestId,
        string providerCode,
        string articleId,
        IList<Tag>? options = null) =>
        await CreateMessage()
            .Write(RequestCode.RequestNewsArticle, requestId)
            .Write(providerCode, articleId, options)
            .SendAsync().ConfigureAwait(false);

    public async ValueTask RequestNewsProvidersAsync() => 
        await CreateMessage().Write(RequestCode.RequestNewsProviders).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestHistoricalNewsAsync(
        int requestId,
        int contractId,
        string providerCodes,
        string startTime,
        string endTime,
        int totalResults,
        IList<Tag>? options = null) => 
        await CreateMessage()
            .Write(RequestCode.RequestHistoricalNews, requestId)
            .Write(contractId, providerCodes, startTime, endTime, totalResults, options)
            .SendAsync().ConfigureAwait(false);

    public async ValueTask RequestHeadTimestampAsync(
        int requestId,
        Contract contract,
        string whatToShow,
        int useRth,
        int formatDate)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.RequestHeadTimestamp, requestId)
            .WriteContract(contract)
            .Write(true) // includeExpired
            .Write(useRth, whatToShow, formatDate)
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask RequestHistogramDataAsync(
        int requestId,
        Contract contract,
        bool useRth,
        string period)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.RequestHistogramData, requestId)
            .WriteContract(contract)
            .Write(true) // includeExpired
            .Write(useRth, period)
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelHistogramDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelHistogramData, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelHeadTimestampAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelHeadTimestamp, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestMarketRuleAsync(int marketRuleId) =>
        await CreateMessage().Write(RequestCode.RequestMarketRule, marketRuleId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestPnLAsync(int requestId, string account, string modelCode) => 
        await CreateMessage().Write(RequestCode.ReqPnL, requestId, account, modelCode).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelPnLAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelPnL, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestPnLSingleAsync(int requestId, string account, string modelCode, int contractId) =>
        await CreateMessage().Write(RequestCode.ReqPnLSingle, requestId, account, modelCode, contractId).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelPnLSingleAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelPnLSingle, requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestHistoricalTicksAsync(
        int requestId,
        Contract contract,
        string startDateTime,
        string endDateTime,
        int numberOfTicks,
        string whatToShow,
        int useRth,
        bool ignoreSize,
        IList<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.ReqHistoricalTicks, requestId)
            .WriteContract(contract)
            .Write(true) // includeExpired
            .Write(startDateTime, endDateTime, numberOfTicks, whatToShow, useRth, ignoreSize)
            .Write(options)
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask RequestTickByTickDataAsync(
        int requestId,
        Contract contract,
        string tickType,
        int numberOfTicks,
        bool ignoreSize)
    {
        ArgumentNullException.ThrowIfNull(contract);
        await CreateMessage()
            .Write(RequestCode.ReqTickByTickData, requestId)
            .WriteContract(contract)
            .Write(tickType, numberOfTicks, ignoreSize)
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelTickByTickDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelTickByTickData, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestCompletedOrdersAsync(bool apiOnly) => 
        await CreateMessage().Write(RequestCode.ReqCompletedOrders, apiOnly).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestWshMetaDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.ReqWshMetaData, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask CancelWshMetaDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelWshMetaData, requestId).SendAsync().ConfigureAwait(false);

    public async ValueTask RequestWshEventDataAsync(int requestId, WshEventData wshEventData)
    {
        ArgumentNullException.ThrowIfNull(wshEventData);
        await CreateMessage()
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
            .SendAsync().ConfigureAwait(false);
    }

    public async ValueTask CancelWshEventDataAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.CancelWshEventData, requestId).SendAsync().ConfigureAwait(false);
    public async ValueTask RequestUserInformationAsync(int requestId) => 
        await CreateMessage().Write(RequestCode.ReqUserInfo, requestId).SendAsync().ConfigureAwait(false);
}

file static class RequestExtensions
{
    extension(double val)
    {
        internal string ToMax()
        {
            if (val == double.MaxValue)
                return "";
            if (val == double.PositiveInfinity)
                return ("Infinity");
            return val.ToString(CultureInfo.InvariantCulture);
        }
    }

    extension(int val)
    {
        internal string ToMax()
        {
            if (val == int.MaxValue)
                return "";
            return val.ToString(CultureInfo.InvariantCulture);
        }
    }
}
