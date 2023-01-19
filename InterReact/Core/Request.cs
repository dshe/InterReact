﻿using NodaTime.Text;
using Stringification;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace InterReact;

/// <summary>
/// Methods which send request messages to TWS/Gateway.
/// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
/// </summary>
public sealed class Request
{
    private readonly InterReactClientConnector Connector;
    private readonly Func<RequestMessage> CreateMessage;

    public Request(InterReactClientConnector connector, Func<RequestMessage> requestMessageFactory)
    {
        ArgumentNullException.ThrowIfNull(connector);
        Connector = connector!;
        CreateMessage = requestMessageFactory;
    }

    /// <summary>
    /// Returns successive ids to uniquely identify requests and orders.
    /// The initial value is set during connection and may start with greater than 0 in case there are previous orders.
    /// </summary>
    public int GetNextId() => Interlocked.Increment(ref Connector.Id);
   
    /// <summary>
    /// Call this method to request market data, streaming or snapshot.
    /// Returns market data for an instrument either in real time or 10-15 minutes delayed (depending on the market data type specified)
    /// Include the generic Tick RealtimeVolume to receive TickRealTimeVolume ticks.
    /// </summary>
    public void RequestMarketData(int requestId, Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null,
        bool isSnapshot = false, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
 
        RequestMessage m = CreateMessage();
        m.Write(RequestCode.RequestMarketData, "11", requestId,
            contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
            contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
            contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass);

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

        m.Write(MakeGenericTicksList(genericTickTypes));
        m.Write(isSnapshot);

        if (Connector.SupportsServerVersion(ServerVersion.SMART_COMPONENTS))
            m.Write(isRegulatorySnapshot); // $.01
        m.Write(Tag.Combine(options)).Send();

        // local
        static string MakeGenericTicksList(IEnumerable<GenericTickType>? genericTickTypes)
        {
            if (genericTickTypes is null || !genericTickTypes.Any())
                return "";
            return genericTickTypes
                .Cast<Enum>()
                .Select(Convert.ToInt32)
                .Select(n => n.ToString(CultureInfo.InvariantCulture))
                .JoinStrings(",");
        }
    }

    public void CancelMarketData(int requestId) => CreateMessage().Write(RequestCode.CancelMarketData, "1", requestId).Send();

    public void PlaceOrder(int orderId, Order order, Contract contract) // the monster
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(contract);

        VerifyOrder(order);

        RequestMessage m = CreateMessage();

        m.Write(RequestCode.PlaceOrder);

        if (!Connector.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
            m.Write("45");

        m.Write(orderId, contract.ContractId);
        m.Write(contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
            contract.Strike, contract.Right, contract.Multiplier, contract.Exchange, contract.PrimaryExchange,
            contract.Currency, contract.LocalSymbol, contract.TradingClass,
            contract.SecurityIdType, contract.SecurityId, order.OrderAction);
        m.Write(order.TotalQuantity);
        m.Write(order.OrderType,
            order.LimitPrice, order.AuxPrice,
            order.TimeInForce, order.OcaGroup,
            order.Account, order.OpenClose, order.Origin,
            order.OrderRef, order.Transmit, order.ParentId,
            order.BlockOrder, order.SweepToFill, order.DisplaySize,
            order.TriggerMethod, order.OutsideRegularTradingHours, order.Hidden);

        if (contract.SecurityType == SecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange,
                    leg.OpenClose, leg.ComboShortSaleSlot, leg.DesignatedLocation, leg.ExemptCode);
            m.Write(order.ComboLegs.Count);
            foreach (OrderComboLeg leg in order.ComboLegs)
                m.Write(leg.Price);
            m.Write(order.SmartComboRoutingParams.Count);
            foreach (Tag tag in order.SmartComboRoutingParams)
                m.Write(tag.Name, tag.Value);
        }

        m.Write("", // deprecated SharesAllocation field
            order.DiscretionaryAmount, order.GoodAfterTime, order.GoodUntilDate,
            order.FinancialAdvisorGroup, order.FinancialAdvisorMethod,
            order.FinancialAdvisorPercentage, order.FinancialAdvisorProfile);

        if (Connector.SupportsServerVersion(ServerVersion.MODELS_SUPPORT))
            m.Write(order.ModelCode);

        m.Write(order.ShortSaleSlot, order.DesignatedLocation, order.ExemptCode,
            order.OcaType, order.Rule80A,
            order.SettlingFirm, order.AllOrNone,
            order.MinimumQuantity, order.PercentOffset,
            false, false, double.MaxValue, //order.ElectronicTradeOnly, order.FirmQuoteOnly, order.NbboPriceCap, 
            order.AuctionStrategy, order.StartingPrice, order.StockReferencePrice,
            order.Delta, order.StockRangeLower, order.StockRangeUpper,
            order.OverridePercentageConstraints, order.Volatility, order.VolatilityType,
            order.DeltaNeutralOrderType, order.DeltaNeutralAuxPrice);

        if (!string.IsNullOrEmpty(order.DeltaNeutralOrderType))
        {
            m.Write(order.DeltaNeutralContractId, order.DeltaNeutralSettlingFirm,
                order.DeltaNeutralClearingAccount, order.DeltaNeutralClearingIntent,
                order.DeltaNeutralOpenClose, order.DeltaNeutralShortSale, order.DeltaNeutralShortSaleSlot,
                order.DeltaNeutralDesignatedLocation);
        }

        m.Write(order.ContinuousUpdate, order.ReferencePriceType,
            order.TrailingStopPrice, order.TrailingStopPercent,
            order.ScaleInitLevelSize, order.ScaleSubsLevelSize, order.ScalePriceIncrement);

        if (order.ScalePriceIncrement > 0 && order.ScalePriceIncrement != double.MaxValue)
        {
            m.Write(order.ScalePriceAdjustValue, order.ScalePriceAdjustInterval,
                order.ScaleProfitOffset, order.ScaleAutoReset, order.ScaleInitPosition,
                order.ScaleInitFillQty, order.ScaleRandomPercent);
        }

        m.Write(order.ScaleTable, order.ActiveStartTime, order.ActiveStopTime, order.HedgeType);

        if (order.HedgeType != HedgeType.Undefined)
            m.Write(order.HedgeParam);

        m.Write(order.OptOutSmartRouting, order.ClearingAccount, order.ClearingIntent, order.NotHeld);

        m.Write(contract.DeltaNeutralContract is not null);
        if (contract.DeltaNeutralContract is not null)
        {
            m.Write(contract.DeltaNeutralContract.ContractId, contract.DeltaNeutralContract.Delta,
                contract.DeltaNeutralContract.Price);
        }

        m.Write(order.AlgoStrategy);
        if (!string.IsNullOrEmpty(order.AlgoStrategy))
        {
            m.Write(order.AlgoParams.Count);
            foreach (Tag tag in order.AlgoParams)
                m.Write(tag.Name, tag.Value);
        }

        m.Write(order.AlgoId, order.WhatIf, Tag.Combine(order.MiscOptions),
            order.Solicited, order.RandomizeSize, order.RandomizePrice);

        if (Connector.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
        {
            if (order.OrderType == OrderType.PeggedToBenchmark)
            {
                m.Write(order.ReferenceContractId, order.IsPeggedChangeAmountDecrease,
                    order.PeggedChangeAmount, order.ReferenceChangeAmount, order.ReferenceExchange);
            }
            m.Write(order.Conditions.Count);
            if (order.Conditions.Count > 0)
            {
                foreach (OrderCondition condition in order.Conditions)
                    condition.Serialize(m);
                m.Write(order.ConditionsIgnoreRegularTradingHours, order.ConditionsCancelOrder);
            }
            m.Write(order.AdjustedOrderType, order.TriggerPrice, order.LmtPriceOffset, order.AdjustedStopPrice,
                order.AdjustedStopLimitPrice, order.AdjustedTrailingAmount, order.AdjustableTrailingUnit);
        }

        if (Connector.SupportsServerVersion(ServerVersion.EXT_OPERATOR))
            m.Write(order.ExtOperator);

        if (Connector.SupportsServerVersion(ServerVersion.SOFT_DOLLAR_TIER))
            m.Write(order.SoftDollarTier.Name, order.SoftDollarTier.Value);

        if (Connector.SupportsServerVersion(ServerVersion.CASH_QTY))
            m.Write(order.CashQty);

        if (Connector.SupportsServerVersion(ServerVersion.DECISION_MAKER))
            m.Write(order.Mifid2DecisionMaker, order.Mifid2DecisionAlgo);

        if (Connector.SupportsServerVersion(ServerVersion.MIFID_EXECUTION))
            m.Write(order.Mifid2ExecutionTrader, order.Mifid2ExecutionAlgo);

        if (Connector.SupportsServerVersion(ServerVersion.AUTO_PRICE_FOR_HEDGE))
            m.Write(order.DontUseAutoPriceForHedge);

        if (Connector.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
            m.Write(order.IsOmsContainer);

        if (Connector.SupportsServerVersion(ServerVersion.D_PEG_ORDERS))
            m.Write(order.DiscretionaryUpToLimitPrice);

        if (Connector.SupportsServerVersion(ServerVersion.PRICE_MGMT_ALGO))
            m.Write(order.UsePriceMgmtAlgo);

        if (Connector.SupportsServerVersion(ServerVersion.DURATION))
            m.Write(order.Duration);

        if (Connector.SupportsServerVersion(ServerVersion.POST_TO_ATS))
            m.Write(order.PostToAts.EncodeNullable());

        m.Send();
    }

    private void VerifyOrder(Order order)
    {
        if (!string.IsNullOrEmpty(order.ExtOperator))
            Connector.RequireServerVersion(ServerVersion.EXT_OPERATOR);

        if (order.CashQty is not null && order.CashQty != double.MaxValue)
            Connector.RequireServerVersion(ServerVersion.CASH_QTY);

        if (!string.IsNullOrEmpty(order.Mifid2DecisionMaker) || !string.IsNullOrEmpty(order.Mifid2DecisionAlgo))
            Connector.RequireServerVersion(ServerVersion.DECISION_MAKER);

        if (!string.IsNullOrEmpty(order.Mifid2ExecutionTrader) || !string.IsNullOrEmpty(order.Mifid2ExecutionAlgo))
            Connector.RequireServerVersion(ServerVersion.DECISION_MAKER);

        if (order.DontUseAutoPriceForHedge)
            Connector.RequireServerVersion(ServerVersion.AUTO_PRICE_FOR_HEDGE);

        if (order.IsOmsContainer)
            Connector.RequireServerVersion(ServerVersion.ORDER_CONTAINER);

        if (order.DiscretionaryUpToLimitPrice)
            Connector.RequireServerVersion(ServerVersion.D_PEG_ORDERS);

        if (order.UsePriceMgmtAlgo.HasValue)
            Connector.RequireServerVersion(ServerVersion.PRICE_MGMT_ALGO);

        if (order.Duration != int.MaxValue && order.Duration is not null)
            Connector.RequireServerVersion(ServerVersion.DURATION);

        if (order.PostToAts != int.MaxValue && order.PostToAts is not null)
            Connector.RequireServerVersion(ServerVersion.POST_TO_ATS);
    }

    public void CancelOrder(int orderId) => CreateMessage().Write(RequestCode.CancelOrder, "1", orderId).Send();

    public void RequestOpenOrders() => CreateMessage().Write(RequestCode.RequestOpenOrders, "1").Send();

    /// <summary>
    /// Call this function to start receiving account updates.
    /// Returns AccountVaue, PortfolioUpdate and TimeUpdate messages.
    /// Updates for all accounts are returned when accountCode is null(?).
    /// This information is updated every three minutes.
    /// </summary>
    public void RequestAccountUpdates(bool start, string? accountCode = null) =>
        CreateMessage().Write(RequestCode.RequestAccountData, "2", start, accountCode).Send();

    /// <summary>
    /// When this method is called, execution reports from the last 24 hours that meet the filter criteria are retrieved.
    /// To view executions beyond the past 24 hours, open the trade log in TWS and, while the Trade Log is displayed, select the days to be returned.
    /// When ExecutionFilter is null, all executions are returned.
    /// Note that the valid format for time is "yyyymmdd-hh:mm:ss"
    /// </summary>
    public void RequestExecutions(int requestId, ExecutionFilter? filter = null) =>
        CreateMessage()
            .Write(RequestCode.RequestExecutions, "3", requestId,
                filter?.ClientId, filter?.Account,
                filter is null ? null : LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd-HH:mm:ss").Format(filter.Time),
                filter?.Symbol, filter?.SecurityType,
                filter?.Exchange, filter?.Side)
            .Send();

    /// <summary>
    /// Call this function to request the next available order Id.
    /// (the numids parameter has been deprecated)
    /// </summary>
    public void RequestNextOrderId() => CreateMessage().Write(RequestCode.RequestNextOrderId, "1", -1).Send();

    /// <summary>
    /// Call this method to retrieve one or more ContractDetails objects for the specified selector contract.
    /// </summary>
    public void RequestContractDetails(int requestId, Contract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        CreateMessage()
            .Write(RequestCode.RequestContractDetails, "8", requestId,
                contract.ContractId,
                contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier,
                contract.Exchange, contract.PrimaryExchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass,
                contract.IncludeExpired, contract.SecurityIdType, contract.SecurityId)
            .Send();
    }

    /// <summary>
    /// Call this method to request market depth for the specified contract. 
    /// </summary>
    public void RequestMarketDepth(int requestId, Contract contract, int numRows = 3, bool isSmartDepth = false, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (isSmartDepth)
            Connector.RequireServerVersion(ServerVersion.SMART_DEPTH);

        if (!string.IsNullOrEmpty(contract.PrimaryExchange))
            Connector.RequireServerVersion(ServerVersion.MKT_DEPTH_PRIM_EXCHANGE);

        RequestMessage m = CreateMessage();

        m.Write(RequestCode.RequestMarketDepth, "5", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange);

        if (Connector.SupportsServerVersion(ServerVersion.MKT_DEPTH_PRIM_EXCHANGE))
            m.Write(contract.PrimaryExchange);

        m.Write(contract.Currency, contract.LocalSymbol, contract.TradingClass, numRows);

        if (Connector.SupportsServerVersion(ServerVersion.SMART_DEPTH))
            m.Write(isSmartDepth);

        m.Write(Tag.Combine(options));

        m.Send();
    }

    public void CancelMarketDepth(int requestId, bool isSmartDepth = false)
    {
        if (isSmartDepth)
            Connector.RequireServerVersion(ServerVersion.SMART_DEPTH);

        RequestMessage m = CreateMessage().Write(RequestCode.CancelMarketDepth, "1", requestId);

        if (Connector.SupportsServerVersion(ServerVersion.SMART_DEPTH))
            m.Write(isSmartDepth);

        m.Send();
    }

    /// <summary>
    /// Call this method to start receiving news bulletins, such as information about exchange status.
    /// </summary>
    public void RequestNewsBulletins(bool all = true) =>
        CreateMessage().Write(RequestCode.RequestNewsBulletins, "1", all).Send();

    public void CancelNewsBulletins() => CreateMessage().Write(RequestCode.CancelNewsBulletins, "1").Send();

    public void ChangeServerLogLevel(LogEntryLevel logLevel) =>
        CreateMessage().Write(RequestCode.ChangeServerLogLevel, "1", logLevel).Send();

    public void RequestAutoOpenOrders(bool autoBind) =>
        CreateMessage().Write(RequestCode.RequestAutoOpenOrders, "1", autoBind).Send();

    public void RequestAllOpenOrders() =>
        CreateMessage().Write(RequestCode.RequestAllOpenOrders, "1").Send();

    public void RequestManagedAccounts() =>
        CreateMessage().Write(RequestCode.RequestManagedAccounts, "1").Send();

    public void RequestFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType) =>
        CreateMessage().Write(RequestCode.RequestFinancialAdvisorConfiguration, "1", dataType).Send();

    public void ReplaceFinancialAdvisorConfiguration(int requestId, FinancialAdvisorDataType dataType, string xml)
    {
        RequestMessage m = CreateMessage().Write(RequestCode.ReplaceFinancialAdvisorConfiguration, "1", dataType, xml);

        if (Connector.SupportsServerVersion(ServerVersion.REPLACE_FA_END))
            m.Write(requestId); // ???

        m.Send();
    }

    /// <summary>
    /// Call this method to receive historical data.
    /// When the "end" argument is null, data up until the current time is returned.
    /// Historical data requests are limited to 60 every 10 minutes.
    /// Errors may still occur when six or more historical data requests for the same Contract, Exchange and Tick Type are made within two seconds.
    /// Note that formatData parameter affects intra-day bars only. 1-day bars always return with date in YYYYMMDD format.
    /// Use Utility.Time.HistoricalTimespan to convert timepsans to strings.
    /// </summary>
    public void RequestHistoricalData(int requestId,
        Contract contract,
        Instant endDate = default,
        HistoricalDuration? duration = null,
        HistoricalBarSize? barSize = null,
        HistoricalDataType? dataType = null,
        bool regularTradingHoursOnly = false,
        bool keepUpToDate = false,
        IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (endDate == default && !keepUpToDate)
            endDate = Connector.Clock.GetCurrentInstant();

        RequestMessage m = CreateMessage().Write(RequestCode.RequestHistoricalData);

        if (!Connector.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
            m.Write("6");

        m.Write(requestId, contract.ContractId, contract.Symbol, contract.SecurityType,
                contract.LastTradeDateOrContractMonth, contract.Strike, contract.Right,
                contract.Multiplier, contract.Exchange, contract.PrimaryExchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass, contract.IncludeExpired,
                InstantPattern.CreateWithInvariantCulture("yyyyMMdd HH:mm:ss").Format(endDate) + " GMT",
                barSize ?? HistoricalBarSize.OneHour,
                duration ?? HistoricalDuration.OneDay,
                regularTradingHoursOnly,
                dataType ?? HistoricalDataType.Trades,
                "1"); // return date as text

        if (contract.SecurityType == SecurityType.Bag)
        {
            m.Write(contract.ComboLegs.Count);
            foreach (ContractComboLeg leg in contract.ComboLegs)
                m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange);
        }

        if (Connector.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
            m.Write(keepUpToDate);

        m.Write(Tag.Combine(options)).Send();
    }

    public void ExerciseOptions(int requestId, Contract contract, OptionExerciseAction exerciseAction,
        int exerciseQuantity, string account, bool overrideOrder)
    {
        ArgumentNullException.ThrowIfNull(contract);

        CreateMessage()
            .Write(RequestCode.ExerciseOptions, "2", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass,
                exerciseAction, exerciseQuantity, account, overrideOrder)
            .Send();
    }

    /// <summary>
    /// Call this method to start receiving market scanner results.
    /// </summary>
    public void RequestScannerSubscription(int requestId, ScannerSubscription subscription, IEnumerable<Tag>? subscriptionOptions = null, IEnumerable<Tag>? subscriptionFilterOptions = null)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        RequestMessage m = CreateMessage().Write(RequestCode.RequestScannerSubscription);
        if (!Connector.SupportsServerVersion(ServerVersion.SCANNER_GENERIC_OPTS))
            m.Write("4");

        m.Write(requestId, subscription.NumberOfRows, subscription.Instrument,
           subscription.LocationCode, subscription.ScanCode,
           subscription.AbovePrice, subscription.BelowPrice, subscription.AboveVolume,
           subscription.MarketCapAbove, subscription.MarketCapBelow,
           subscription.MoodyRatingAbove, subscription.MoodyRatingBelow,
           subscription.SpRatingAbove, subscription.SpRatingBelow,
           subscription.MaturityDateAbove, subscription.MaturityDateBelow,
           subscription.CouponRateAbove, subscription.CouponRateBelow,
           subscription.ExcludeConvertible, subscription.AverageOptionVolumeAbove,
           subscription.ScannerSettingPairs, subscription.StockType);

        if (Connector.SupportsServerVersion(ServerVersion.SCANNER_GENERIC_OPTS))
            m.Write(Tag.Combine(subscriptionFilterOptions));

        m.Write(Tag.Combine(subscriptionOptions));
        m.Send();
    }

    public void CancelScannerSubscription(int requestId) =>
        CreateMessage().Write(RequestCode.CancelScannerSubscription, "1", requestId).Send();

    /// <summary>
    /// Call the this method to receive an XML document that describes the valid parameters that a scanner subscription can have.
    /// </summary>ca
    public void RequestScannerParameters() => CreateMessage().Write(RequestCode.RequestScannerParameters, "1").Send();

    public void CancelHistoricalData(int requestId) =>
        CreateMessage().Write(RequestCode.CancelHistoricalData, "1", requestId).Send();

    /// <summary>
    /// This is the time reported by TWS. Seconds precision.
    /// </summary>
    public void RequestCurrentTime() => CreateMessage().Write(RequestCode.RequestCurrentTime, 1).Send();

    /// <summary>
    /// Call this method to start receiving realtime bar data.
    /// </summary>
    public void RequestRealTimeBars(int requestId, Contract contract, RealtimeBarType? whatToShow = null,
        bool regularTradingHoursOnly = false, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        CreateMessage()
            .Write(RequestCode.RequestRealtimeBars, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                "5", // only 5 seconds bars are available
                whatToShow ?? RealtimeBarType.Trades,
                regularTradingHoursOnly, Tag.Combine(options))
            .Send();
    }

    public void CancelRealTimeBars(int requestId) => CreateMessage().Write(RequestCode.CancelRealtimeBars, "1", requestId).Send();

    /// <summary>
    /// Call this method to receive Reuters global fundamental data for stocks.
    /// There must be a subscription to Reuters Fundamental set up in Account Management before you can receive this data.
    /// IB: The method can handle conid specified in the Contract object, but not tradingClass or multiplier.
    /// </summary>
    public void RequestFundamentalData(int requestId, Contract contract, FundamentalDataReportType? reportTypeN = null, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        FundamentalDataReportType reportType = reportTypeN ?? FundamentalDataReportType.CompanyOverview;
        CreateMessage()
            .Write(RequestCode.RequestFundamentalData, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol,
                reportType.ToString(), Tag.Combine(options))
            .Send();
    }

    public void CancelFundamentalData(int requestId) => CreateMessage().Write(RequestCode.CancelFundamentalData, "1", requestId).Send();

    /// <summary>
    /// Call this function to calculate volatility for a supplied option price and underlying price.
    /// </summary>
    public void CalculateImpliedVolatility(int requestId, Contract contract, double optionPrice, double underlyingPrice, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        CreateMessage()
            .Write(RequestCode.RequestCalculatedImpliedVolatility, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                optionPrice, underlyingPrice, Tag.Combine(options))
            .Send();
    }

    /// <summary>
    /// Call this function to calculate option price and greek Values for a supplied volatility and underlying price.
    /// </summary>
    public void CalculateOptionPrice(int requestId, Contract contract, double volatility, double underlyingPrice, IEnumerable<Tag>? options = null)
    {
        ArgumentNullException.ThrowIfNull(contract);

        CreateMessage()
            .Write(RequestCode.RequestCalculatedOptionPrice, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                volatility, underlyingPrice, Tag.Combine(options))
            .Send();
    }

    public void CancelCalculateImpliedVolatility(int requestId) =>
        CreateMessage().Write(RequestCode.CancelCalculatedImpliedVolatility, "1", requestId).Send();

    public void CancelCalculateOptionPrice(int requestId) =>
        CreateMessage().Write(RequestCode.CancelCalculatedOptionPrice, "1", requestId).Send();

    /// <summary>
    /// Use this method to cancel all open orders globally. It cancels both API and TWS open orders.
    /// </summary>
    public void RequestGlobalCancel() => CreateMessage().Write(RequestCode.RequestGlobalCancel, "1").Send();

    /// <summary>
    /// The API can receive frozen market data from TWS. Frozen market data is the last data recorded in IB's system.
    /// During normal trading hours, the API receives real-time market data.
    /// If you use this function, you are telling TWS to automatically switch to frozen market data after the close.
    /// Then, before the opening of the next trading day, market data will automatically switch back to real-time market data.
    /// </summary>
    public void RequestMarketDataType(MarketDataType marketDataType) =>
        CreateMessage().Write(RequestCode.RequestMarketDataType, "1", marketDataType).Send();

    /// <summary>
    /// Subscribes to position updates for all accessible accounts. All positions sent initially, and then only updates as positions change. 
    /// </summary>
    public void RequestPositions() => CreateMessage().Write(RequestCode.RequestPositions, "1").Send();

    /// <summary>
    /// Subscribea to data that appears on the TWS Account Window Summary tab. 
    /// If no tags are specified, data for all tags will be requested.
    /// </summary>
    public void RequestAccountSummary(int requestId, string group = "All", IEnumerable<AccountSummaryTag>? tags = null)
    {
        List<string>? tagNames = tags?.Select(tag => tag.ToString()).ToList();
        if (tagNames is null || !tagNames.Any())
            tagNames = Enum.GetNames<AccountSummaryTag>().ToList();
        CreateMessage()
            .Write(RequestCode.RequestAccountSummary, "1", requestId, group, string.Join(",", tagNames))
            .Send();
    }

    public void CancelAccountSummary(int requestId) =>
        CreateMessage().Write(RequestCode.CancelAccountSummary, "1", requestId).Send();

    public void CancelPositions() => CreateMessage().Write(RequestCode.CancelPositions, "1").Send();

    // 65, 66: For IB internal use

    public void QueryDisplayGroups(int requestId) =>
        CreateMessage().Write(RequestCode.QueryDisplayGroups, "1", requestId).Send();

    public void SubscribeToGroupEvents(int requestId, int groupId) =>
        CreateMessage().Write(RequestCode.SubscribeToGroupEvents, "1", requestId, groupId).Send();

    public void UpdateDisplayGroup(int requestId, string contractInfo) =>
        CreateMessage().Write(RequestCode.UpdateDisplayGroup, "1", requestId, contractInfo).Send();

    public void UnsubscribeFromGroupEvents(int requestId) =>
        CreateMessage().Write(RequestCode.UnsubscribeFromGroupEvents, "1", requestId).Send();

    // 71: StartApi
    // 72, 73: for IB internal use

    /**
    * Requests position subscription for account and/or model
    * Initially all positions are returned, and then updates are returned for any position changes in real time.
    * If an account Id is provided, only the account's positions belonging to the specified model will be delivered
    */
    public void RequestPositionsMulti(int requestId, string account, string modelCode)
    {
        Connector.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
        CreateMessage().Write(RequestCode.RequestPositionsMulti, "1", requestId, account, modelCode).Send();
    }

    public void CancelPositionsMulti(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
        CreateMessage().Write(RequestCode.CancelPositionsMulti, "1", requestId).Send();
    }

    public void RequestAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNlv)
    {
        Connector.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
        CreateMessage().Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId, account, modelCode, ledgerAndNlv).Send();
    }

    public void CancelAccountUpdatesMulti(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
        CreateMessage().Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId).Send();
    }

    public void RequestSecDefOptParams(int requestId, string underlyingSymbol, string futFopExchange, string underlyingSecType, int underlyingConId)
    {
        Connector.RequireServerVersion(ServerVersion.SEC_DEF_OPT_PARAMS_REQ);
        CreateMessage()
            .Write(RequestCode.RequestSecurityDefinitionOptionalParameters, requestId,
                underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId)
            .Send();
    }

    public void RequestSoftDollarTiers(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.SOFT_DOLLAR_TIER);
        CreateMessage()
            .Write(RequestCode.RequestSoftDollarTiers, requestId)
            .Send();
    }

    public void RequestFamilyCodes()
    {
        Connector.RequireServerVersion(ServerVersion.REQ_FAMILY_CODES);
        CreateMessage().Write(RequestCode.RequestFamilyCodes).Send();
    }

    public void RequestMatchingSymbols(int requestId, string pattern)
    {
        Connector.RequireServerVersion(ServerVersion.REQ_MATCHING_SYMBOLS);
        CreateMessage().Write(RequestCode.RequestMatchingSymbols, requestId, pattern).Send();
    }

    // Discover exchhanges for which market data is returned to updateMktDepthL2(those with market makers)
    public void RequestMarketDepthExchanges()
    {
        Connector.RequireServerVersion(ServerVersion.REQ_MKT_DEPTH_EXCHANGES);
        CreateMessage().Write(RequestCode.RequestMktDepthExchanges).Send();
    }

    // Returns the mapping of single letter codes to exchange names given the mapping identifier
    public void RequestSmartComponents(int requestId, string bboExchange)
    {
        Connector.RequireServerVersion(ServerVersion.REQ_MKT_DEPTH_EXCHANGES);
        CreateMessage().Write(RequestCode.RequestSmartComponents, requestId, bboExchange).Send();
    }

    // retrieve the body of a news article
    public void RequestNewsArticle(int requestId, string providerCode, string articleId, IEnumerable<Tag>? options = null)
    {
        Connector.RequireServerVersion(ServerVersion.REQ_NEWS_ARTICLE);

        RequestMessage m = CreateMessage().Write(RequestCode.RequestNewsArticle, requestId, providerCode, articleId);
        if (Connector.SupportsServerVersion(ServerVersion.NEWS_QUERY_ORIGINS))
            m.Write(Tag.Combine(options));
        m.Send();
    }

    public void RequestNewsProviders()
    {
        Connector.RequireServerVersion(ServerVersion.REQ_NEWS_PROVIDERS);
        CreateMessage().Write(RequestCode.RequestNewsProviders).Send();
    }

    public void RequestHistoricalNews(int requestId, int conId, string providerCodes, string startTime, string endTime, int totalResults, IEnumerable<Tag>? options = null)
    {
        Connector.RequireServerVersion(ServerVersion.REQ_HISTORICAL_NEWS);
        RequestMessage m = CreateMessage().Write(RequestCode.RequestHistoricalNews, requestId, conId, providerCodes, startTime, endTime, totalResults);
        if (Connector.SupportsServerVersion(ServerVersion.NEWS_QUERY_ORIGINS))
            m.Write(Tag.Combine(options));
        m.Send();
    }

    // Returns the timestamp of earliest available historical data for a contract and data type
    // whatToShow - type of data for head timestamp - "BID", "ASK", "TRADES", etc
    // useRTH - use regular trading hours only, 1 for yes or 0 for no
    // formatDate - @param formatDate set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds
    public void RequestHeadTimestamp(int requestId, Contract contract, string whatToShow, int useRth, int formatDate)
    {
        ArgumentNullException.ThrowIfNull(contract);

        Connector.RequireServerVersion(ServerVersion.REQ_HEAD_TIMESTAMP);
        CreateMessage()
            .Write(RequestCode.RequestHeadTimestamp, requestId)
            .WriteContract(contract)
            .Write(useRth, whatToShow, formatDate)
            .Send();
    }

    // Returns data histogram of specified contract
    public void RequestHistogramData(int requestId, Contract contract, bool useRth, string period)
    {
        ArgumentNullException.ThrowIfNull(contract);

        Connector.RequireServerVersion(ServerVersion.REQ_HISTOGRAM_DATA);
        CreateMessage()
            .Write(RequestCode.RequestHistogramData, requestId)
            .WriteContract(contract)
            .Write(useRth, period)
            .Send();
    }

    public void CancelHistogramData(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.REQ_HISTOGRAM_DATA);
        CreateMessage().Write(RequestCode.CancelHistogramData, requestId).Send();
    }

    public void CancelHeadTimestamp(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.CANCEL_HEADTIMESTAMP);
        CreateMessage().Write(RequestCode.CancelHeadTimestamp, requestId).Send();
    }

    // Find market rule (minimum price increment) from codes indicated in ContractDetails.
    public void RequestMarketRule(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.MARKET_RULES);
        CreateMessage().Write(RequestCode.RequestMarketRule, requestId).Send();
    }

    public void RequestPnL(int requestId, string account, string modelCode)
    {
        Connector.RequireServerVersion(ServerVersion.PNL);
        CreateMessage().Write(RequestCode.ReqPnL, requestId, account, modelCode).Send();
    }

    public void CancelPnL(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.PNL);
        CreateMessage().Write(RequestCode.CancelPnL, requestId).Send();
    }

    public void RequestPnLSingle(int requestId, string account, string modelCode, int contractId)
    {
        Connector.RequireServerVersion(ServerVersion.PNL);
        CreateMessage().Write(RequestCode.ReqPnLSingle, requestId, account, modelCode, contractId).Send();
    }

    public void CancelPnLSingle(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.PNL);
        CreateMessage().Write(RequestCode.CancelPnLSingle, requestId).Send();
    }

    public void RequestHistoricalTicks(int requestId, Contract contract, string startDateTime, string endDateTime,
        int numberOfTicks, string whatToShow, int useRth, bool ignoreSize, IEnumerable<Tag> miscOptions)
    {
        Connector.RequireServerVersion(ServerVersion.HISTORICAL_TICKS);
        CreateMessage().Write(RequestCode.ReqHistoricalTicks, requestId, contract,
            startDateTime, endDateTime, numberOfTicks, whatToShow, useRth, ignoreSize, miscOptions).Send();
    }

    // Request high Resolution Historical Data
    public void RequestTickByTickData(int requestId, Contract contract, string tickType, int numberOfTicks, bool ignoreSize)
    {
        ArgumentNullException.ThrowIfNull(contract);

        Connector.RequireServerVersion(ServerVersion.TICK_BY_TICK);
        if (numberOfTicks != 0 || ignoreSize)
            Connector.RequireServerVersion(ServerVersion.TICK_BY_TICK_IGNORE_SIZE);

        RequestMessage m = CreateMessage().Write(RequestCode.ReqTickByTickData, requestId,
            contract.ContractId,
            contract.Symbol,
            contract.SecurityType,
            contract.LastTradeDateOrContractMonth,
            contract.Strike,
            contract.Right,
            contract.Multiplier,
            contract.Exchange,
            contract.PrimaryExchange,
            contract.Currency,
            contract.LocalSymbol,
            contract.TradingClass,
            tickType);

        if (Connector.SupportsServerVersion(ServerVersion.TICK_BY_TICK_IGNORE_SIZE))
            m.Write(numberOfTicks, ignoreSize);

        m.Send();
    }

    public void CancelTickByTickData(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.TICK_BY_TICK);
        CreateMessage().Write(RequestCode.CancelTickByTickData, requestId).Send();
    }

    public void RequestCompletedOrders(bool apiOnly)
    {
        Connector.RequireServerVersion(ServerVersion.COMPLETED_ORDERS);
        CreateMessage().Write(RequestCode.ReqCompletedOrders, apiOnly).Send();
    }

    // Request fundamental data from the Wall Street Events Horizon Calendar
    public void RequestWshMetaData(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.WSHE_CALENDAR);
        CreateMessage().Write(RequestCode.ReqWshMetaData, requestId).Send();
    }

    public void CancelWshMetaData(int requestId)
    {

        Connector.RequireServerVersion(ServerVersion.WSHE_CALENDAR);
        CreateMessage().Write(RequestCode.CancelWshMetaData, requestId).Send();
    }

    public void RequestWshEventData(int requestId, int contractId)
    {
        Connector.RequireServerVersion(ServerVersion.WSHE_CALENDAR);
        CreateMessage().Write(RequestCode.ReqWshEventData, requestId, contractId).Send();
    }

    public void CancelWshEventData(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.WSHE_CALENDAR);
        CreateMessage().Write(RequestCode.CancelWshEventData, requestId).Send();
    }

    public void RequestUserInformation(int requestId)
    {
        Connector.RequireServerVersion(ServerVersion.USER_INFO);
        CreateMessage().Write(RequestCode.RequestUserInfo, requestId).Send();
    }
}
