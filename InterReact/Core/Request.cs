using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NodaTime;
using NodaTime.Text;
using RxSockets;
using Stringification;

namespace InterReact
{
    /// <summary>
    /// Methods which send request messages to TWS/Gateway.
    /// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
    /// </summary>
    public class Request : IEditorBrowsableNever
    {
        public Config Config { get; }
        private readonly IRxSocketClient RxSocket;
        private readonly Limiter Limiter;

        public Request(Config config, IRxSocketClient rxSocket, Limiter limiter)
        {
            Config = config;
            RxSocket = rxSocket;
            Limiter = limiter;
        }

        private RequestMessage Message() => new(RxSocket, Limiter);

        /// <summary>
        /// Returns successive unique ids which are used to uniquely identify requests and orders.
        /// </summary>
        public int GetNextId() => Interlocked.Increment(ref NextIdValue);
        private int NextIdValue;

        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Call this method to request market data. The market data will be returned as Tick* objects.
        /// Include the generic Tick RealtimeVolume to receive TickRealTimeVolume ticks.
        /// </summary>
        public void RequestMarketData(int requestId, Contract contract,
            IEnumerable<GenericTickType>? genericTickTypes = null,
            bool isSnapshot = false, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
        {
            RequestMessage m = Message();

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
            m.Write(dnc != null);
            if (dnc != null)
                m.Write(dnc.ContractId, dnc.Delta, dnc.Price);
            m.Write(MakeGenericTicksList(genericTickTypes));
            m.Write(isSnapshot);
            if (Config.SupportsServerVersion(ServerVersion.SMART_COMPONENTS))
                m.Write(isRegulatorySnapshot); // $.01
            m.Write(Tag.Combine(options)).Send();

            // local
            static string MakeGenericTicksList(IEnumerable<GenericTickType>? genericTickTypes)
            {
                if (genericTickTypes == null || !genericTickTypes.Any())
                    return "";
                return genericTickTypes
                    .Cast<Enum>()
                    .Select(Convert.ToInt32)
                    .Select(n => n.ToString())
                    .JoinStrings(",");
            }
        }

        public void CancelMarketData(int requestId) =>
            Message().Write(RequestCode.CancelMarketData, "1", requestId).Send();

        public void PlaceOrder(int id, Order order, Contract contract) // the monster
        {
            if (!string.IsNullOrEmpty(order.ExtOperator))
                Config.RequireServerVersion(ServerVersion.EXT_OPERATOR);
            if (order.CashQty != null)
                Config.RequireServerVersion(ServerVersion.CASH_QTY);

            RequestMessage m = Message();

            m.Write(RequestCode.PlaceOrder);

            if (!Config.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
                m.Write("45");

            m.Write(id, contract.ContractId);
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

            if (Config.SupportsServerVersion(ServerVersion.MODELS_SUPPORT))
                m.Write(order.ModelCode);

            m.Write(order.ShortSaleSlot, order.DesignatedLocation, order.ExemptCode,
                order.OcaType, order.Rule80A,
                order.SettlingFirm, order.AllOrNone,
                order.MinimumQuantity, order.PercentOffset,
                //order.ElectronicTradeOnly, order.FirmQuoteOnly, order.NbboPriceCap, 
                false, false, double.MaxValue,
                order.AuctionStrategy, order.StartingPrice, order.StockReferencePrice,
                order.Delta, order.StockRangeLower, order.StockRangeUpper,
                order.OverridePercentageConstraints, order.Volatility, order.VolatilityType,
                order.DeltaNeutralOrderType, order.DeltaNeutralAuxPrice);

            if (!string.IsNullOrEmpty(order.DeltaNeutralOrderType))
                m.Write(order.DeltaNeutralContractId, order.DeltaNeutralSettlingFirm,
                    order.DeltaNeutralClearingAccount, order.DeltaNeutralClearingIntent,
                    order.DeltaNeutralOpenClose, order.DeltaNeutralShortSale, order.DeltaNeutralShortSaleSlot,
                    order.DeltaNeutralDesignatedLocation);

            m.Write(order.ContinuousUpdate, order.ReferencePriceType,
                order.TrailingStopPrice, order.TrailingStopPercent,
                order.ScaleInitLevelSize, order.ScaleSubsLevelSize, order.ScalePriceIncrement);

            if (order.ScalePriceIncrement != null)
                m.Write(order.ScalePriceAdjustValue, order.ScalePriceAdjustInterval,
                    order.ScaleProfitOffset, order.ScaleAutoReset, order.ScaleInitPosition,
                    order.ScaleInitFillQty, order.ScaleRandomPercent);

            m.Write(order.ScaleTable, order.ActiveStartTime, order.ActiveStopTime, order.HedgeType);

            if (order.HedgeType != HedgeType.Undefined)
                m.Write(order.HedgeParam);

            m.Write(order.OptOutSmartRouting, order.ClearingAccount, order.ClearingIntent, order.NotHeld);

            m.Write(contract.DeltaNeutralContract != null);
            if (contract.DeltaNeutralContract != null)
                m.Write(contract.DeltaNeutralContract.ContractId, contract.DeltaNeutralContract.Delta,
                    contract.DeltaNeutralContract.Price);

            m.Write(order.AlgoStrategy);
            if (!string.IsNullOrEmpty(order.AlgoStrategy))
            {
                m.Write(order.AlgoParams.Count);
                foreach (Tag tag in order.AlgoParams)
                    m.Write(tag.Name, tag.Value);
            }

            m.Write(order.AlgoId, order.WhatIf, Tag.Combine(order.MiscOptions),
                order.Solicited, order.RandomizeSize, order.RandomizePrice);

            if (Config.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
            {
                if (order.OrderType == OrderType.PeggedToBenchmark)
                    m.Write(order.ReferenceContractId, order.IsPeggedChangeAmountDecrease,
                        order.PeggedChangeAmount, order.ReferenceChangeAmount, order.ReferenceExchange);
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

            if (Config.SupportsServerVersion(ServerVersion.EXT_OPERATOR))
                m.Write(order.ExtOperator);

            if (Config.SupportsServerVersion(ServerVersion.SOFT_DOLLAR_TIER))
                m.Write(order.SoftDollarTier.Name, order.SoftDollarTier.Value);

            if (Config.SupportsServerVersion(ServerVersion.CASH_QTY))
                m.Write(order.CashQty);

            if (Config.SupportsServerVersion(ServerVersion.EXT_OPERATOR))
                m.Write(order.ExtOperator);

            if (Config.SupportsServerVersion(ServerVersion.SOFT_DOLLAR_TIER))
                m.Write(order.SoftDollarTier.Name, order.SoftDollarTier.Value);

            if (Config.SupportsServerVersion(ServerVersion.CASH_QTY))
                m.Write(order.CashQty);

            if (Config.SupportsServerVersion(ServerVersion.DECISION_MAKER))
                m.Write(order.Mifid2DecisionMaker, order.Mifid2DecisionAlgo);

            if (Config.SupportsServerVersion(ServerVersion.MIFID_EXECUTION))
                m.Write(order.Mifid2ExecutionTrader, order.Mifid2ExecutionAlgo);

            if (Config.SupportsServerVersion(ServerVersion.AUTO_PRICE_FOR_HEDGE))
                m.Write(order.DontUseAutoPriceForHedge);

            if (Config.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
                m.Write(order.IsOmsContainer);

            if (Config.SupportsServerVersion(ServerVersion.D_PEG_ORDERS))
                m.Write(order.DiscretionaryUpToLimitPrice);

            if (Config.SupportsServerVersion(ServerVersion.PRICE_MGMT_ALGO))
                m.Write(order.UsePriceMgmtAlgo);

            if (Config.SupportsServerVersion(ServerVersion.DURATION))
                m.Write(order.Duration);

            if (Config.SupportsServerVersion(ServerVersion.POST_TO_ATS))
                m.Write(order.PostToAts);

            m.Send();
        }

        public void CancelOrder(int orderId) => Message().Write(RequestCode.CancelOrder, "1", orderId).Send();

        public void RequestOpenOrders() => Message().Write(RequestCode.RequestOpenOrders, "1").Send();

        /// <summary>
        /// Call this function to start receiving account updates.
        /// Returns AccountVaue, PortfolioUpdate and TimeUpdate messages.
        /// Updates for all accounts are returned when accountCode is null(?).
        /// This information is updated every three minutes.
        /// </summary>
        public void RequestAccountUpdates(bool start, string? accountCode = null) =>
            Message().Write(RequestCode.RequestAccountData, "2", start, accountCode).Send();

        /// <summary>
        /// When this method is called, execution reports from the last 24 hours that meet the filter criteria are retrieved.
        /// To view executions beyond the past 24 hours, open the trade log in TWS and, while the Trade Log is displayed, select the days to be returned.
        /// When ExecutionFilter is null, all executions are returned.
        /// Note that the valid format for time is "yyyymmdd-hh:mm:ss"
        /// </summary>
        public void RequestExecutions(int requestId, ExecutionFilter? filter = null) =>
            Message()
                .Write(RequestCode.RequestExecutions, "3", requestId,
                    filter?.ClientId, filter?.Account,
                    filter == null ? null : LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd-HH:mm:ss").Format(filter.Time),
                    filter?.Symbol, filter?.SecurityType,
                    filter?.Exchange, filter?.Side)
                .Send();

        /// <summary>
        /// In case there are multiple API clients attached to TWS, the OrderId may not be unique among the clients.
        /// Call this function to request an OrderId that can be used with multiple clients.
        /// </summary>
        public void RequestNextOrderId() => Message().Write(RequestCode.RequestIds, "1", "1").Send();

        internal static object[] ContractObjects(Contract contract) =>
            new object[] { 
                    contract.ContractId, 
                    contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                    contract.Strike, contract.Right, contract.Multiplier,
                    contract.Exchange, contract.PrimaryExchange,
                    contract.Currency, contract.LocalSymbol, contract.TradingClass,
                    contract.IncludeExpired, contract.SecurityIdType, contract.SecurityId };

        /// <summary>
        /// Call this method to retrieve one or more ContractDetails objects for the specified selector contract.
        /// </summary>
        public void RequestContractDetails(int requestId, Contract contract) =>
            Message().Write(RequestCode.RequestContractDetails, "8", requestId, ContractObjects(contract)).Send();

        /// <summary>
        /// Call this method to request market depth for the specified contract. 
        /// </summary>
        public void RequestMarketDepth(int requestId, Contract contract, int numRows = 3, IList<Tag>? options = null)
        {
            Message()
                .Write(RequestCode.RequestMarketDepth, "5", requestId,
                    contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                    contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                    contract.Currency, contract.LocalSymbol, contract.TradingClass, numRows,
                    Tag.Combine(options))
                .Send();
        }

        public void CancelMarketDepth(int requestId) => 
            Message().Write(RequestCode.CancelMarketDepth, "1", requestId).Send();

        /// <summary>
        /// Call this method to start receiving news bulletins, such as information about exchange status.
        /// </summary>
        public void RequestNewsBulletins(bool all = true) =>
            Message().Write(RequestCode.RequestNewsBulletins, "1", all).Send();

        public void CancelNewsBulletins() => Message().Write(RequestCode.CancelNewsBulletins, "1").Send();

        public void ChangeServerLogLevel(LogEntryLevel logLevel) =>
            Message().Write(RequestCode.ChangeServerLogLevel, "1", logLevel).Send();

        public void RequestAutoOpenOrders(bool autoBind) =>
            Message().Write(RequestCode.RequestAutoOpenOrders, "1", autoBind).Send();

        public void RequestAllOpenOrders() =>
            Message().Write(RequestCode.RequestAllOpenOrders, "1").Send();

        public void RequestManagedAccounts() =>
            Message().Write(RequestCode.RequestManagedAccounts, "1").Send();

        public void RequestFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType) =>
            Message().Write(RequestCode.RequestFinancialAdvisorConfiguration, "1", dataType).Send();

        public void ReplaceFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType, string xml) =>
            Message().Write(RequestCode.ReplaceFinancialAdvisorConfiguration, "1", dataType, xml).Send();

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
            IList<Tag>? options = null)
        {
            if (endDate == default)
                endDate = Config.Clock.GetCurrentInstant();

            RequestMessage m = Message()
                .Write(RequestCode.RequestHistoricalData, "6",
                    requestId, contract.ContractId, contract.Symbol, contract.SecurityType,
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
            m.Write(Tag.Combine(options)).Send();
        }

        public void ExerciseOptions(int requestId, Contract contract, OptionExerciseAction exerciseAction, int exerciseQuantity,
            string account, bool overrideOrder)
        {
            Message()
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
        public void RequestScannerSubscription(int requestId, ScannerSubscription subscription, IList<Tag>? subscriptionOptions = null)
        {
            Message()
                .Write(RequestCode.RequestScannerSubscription,"4",
                    requestId, subscription.NumberOfRows, subscription.Instrument,
                    subscription.LocationCode, subscription.ScanCode,
                    subscription.AbovePrice, subscription.BelowPrice, subscription.AboveVolume,
                    subscription.MarketCapAbove, subscription.MarketCapBelow,
                    subscription.MoodyRatingAbove, subscription.MoodyRatingBelow,
                    subscription.SpRatingAbove, subscription.SpRatingBelow,
                    subscription.MaturityDateAbove, subscription.MaturityDateBelow,
                    subscription.CouponRateAbove, subscription.CouponRateBelow,
                    subscription.ExcludeConvertible, subscription.AverageOptionVolumeAbove,
                    subscription.ScannerSettingPairs, subscription.StockType,
                    Tag.Combine(subscriptionOptions))
                .Send();
        }

        public void CancelScannerSubscription(int requestId) =>
            Message().Write(RequestCode.CancelScannerSubscription, "1", requestId).Send();

        /// <summary>
        /// Call the this method to receive an XML document that describes the valid parameters that a scanner subscription can have.
        /// </summary>ca
        public void RequestScannerParameters() => Message().Write(RequestCode.RequestScannerParameters, "1").Send();

        public void CancelHistoricalData(int requestId) =>
            Message().Write(RequestCode.CancelHistoricalData, "1", requestId).Send();

        /// <summary>
        /// This is the time reported by TWS. Seconds precision.
        /// </summary>
        public void RequestCurrentTime() => Message().Write(RequestCode.RequestCurrentTime, 1).Send();

        /// <summary>
        /// Call this method to start receiving realtime bar data.
        /// </summary>
        public void RequestRealTimeBars(int requestId, Contract contract, RealtimeBarType? whatToShow = null,
            bool regularTradingHoursOnly = false, IList<Tag>? options = null)
        {
            Message()
                .Write(RequestCode.RequestRealtimeBars, "3", requestId,
                    contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                    contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                    contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                    "5", // only 5 seconds bars are available
                    whatToShow ?? RealtimeBarType.Trades,
                    regularTradingHoursOnly, Tag.Combine(options))
                .Send();
        }

        public void CancelRealTimeBars(int requestId) => Message().Write(RequestCode.CancelRealtimeBars, "1", requestId).Send();

        /// <summary>
        /// Call this method to receive Reuters global fundamental data for stocks.
        /// There must be a subscription to Reuters Fundamental set up in Account Management before you can receive this data.
        /// IB: The method can handle conid specified in the Contract object, but not tradingClass or multiplier.
        /// </summary>
        public void RequestFundamentalData(int requestId, Contract contract, FundamentalDataReportType? reportTypeN = null, IList<Tag>? options = null)
        {
            FundamentalDataReportType reportType = reportTypeN ?? FundamentalDataReportType.CompanyOverview;
            Message()
                .Write(RequestCode.RequestFundamentalData, "3", requestId,
                    contract.ContractId, contract.Symbol, contract.SecurityType, contract.Exchange,
                    contract.PrimaryExchange, contract.Currency, contract.LocalSymbol,
                    reportType.ToString(), Tag.Combine(options))
                .Send();
        }

        public void CancelFundamentalData(int requestId) => Message().Write(RequestCode.CancelFundamentalData, "1", requestId).Send();

        /// <summary>
        /// Call this function to calculate volatility for a supplied option price and underlying price.
        /// </summary>
        public void CalculateImpliedVolatility(int requestId, Contract contract, double optionPrice, double underlyingPrice, IList<Tag>? options = null)
        {
            Message()
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
        public void CalculateOptionPrice(int requestId, Contract contract, double volatility, double underlyingPrice, IList<Tag>? options = null)
        {
            Message()
                .Write(RequestCode.RequestCalculatedOptionPrice, "3", requestId,
                    contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                    contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                    contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                    volatility, underlyingPrice, Tag.Combine(options))
                .Send();
        }

        public void CancelCalculateImpliedVolatility(int requestId) =>
            Message().Write(RequestCode.CancelCalculatedImpliedVolatility, "1", requestId).Send();

        public void CancelCalculateOptionPrice(int requestId) =>
            Message().Write(RequestCode.CancelCalculatedOptionPrice, "1", requestId).Send();

        /// <summary>
        /// Use this method to cancel all open orders globally. It cancels both API and TWS open orders.
        /// </summary>
        public void RequestGlobalCancel() => Message().Write(RequestCode.RequestGlobalCancel, "1").Send();

        /// <summary>
        /// The API can receive frozen market data from TWS. Frozen market data is the last data recorded in IB's system.
        /// During normal trading hours, the API receives real-time market data.
        /// If you use this function, you are telling TWS to automatically switch to frozen market data after the close.
        /// Then, before the opening of the next trading day, market data will automatically switch back to real-time market data.
        /// </summary>
        public void RequestMarketDataType(MarketDataType marketDataType) =>
            Message().Write(RequestCode.RequestMarketDataType, "1", marketDataType).Send();

        /// <summary>
        /// Subscribes to position updates for all accessible accounts. All positions sent initially, and then only updates as positions change. 
        /// </summary>
        public void RequestPositions() => Message().Write(RequestCode.RequestPositions, "1").Send();

        /// <summary>
        /// Subscribea to data that appears on the TWS Account Window Summary tab. 
        /// If no tags are specified, data for all tags will be requested.
        /// </summary>
        public void RequestAccountSummary(int requestId, string group = "All", IList<AccountSummaryTag>? tags = null)
        {
            List<string>? tagNames = tags?.Select(tag => tag.ToString()).ToList();
            if (tagNames == null || !tagNames.Any())
                tagNames = Enum.GetNames(typeof(AccountSummaryTag)).ToList();
            Message()
                .Write(RequestCode.RequestAccountSummary, "1", requestId, group, string.Join(",", tagNames))
                .Send();
        }

        public void CancelAccountSummary(int requestId) =>
            Message().Write(RequestCode.CancelAccountSummary, "1", requestId).Send();

        public void CancelPositions() => Message().Write(RequestCode.CancelPositions, "1").Send();

        // For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
        public void VerifyRequest(string apiName, string apiVersion) =>
            Message().Write(RequestCode.VerifyRequest, "1", apiName, apiVersion).Send();

        //  For IB's internal purpose.Allows to provide means of verification between the TWS and third party programs.
        public void VerifyMessage(string apiData) =>
            Message().Write(RequestCode.VerifyMessage, "1", apiData).Send();

        public void QueryDisplayGroups(int requestId) =>
            Message().Write(RequestCode.QueryDisplayGroups, "1", requestId).Send();

        public void SubscribeToGroupEvents(int requestId, int groupId) =>
            Message().Write(RequestCode.SubscribeToGroupEvents, "1", requestId, groupId).Send();

        public void UpdateDisplayGroup(int requestId, string contractInfo) =>
            Message().Write(RequestCode.UpdateDisplayGroup, "1", requestId, contractInfo).Send();

        public void UnsubscribeFromGroupEvents(int requestId) =>
            Message().Write(RequestCode.UnsubscribeFromGroupEvents, "1", requestId).Send();

        // For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
        public void VerifyAndAuthorizeRequest(string apiName, string apiVersion, string opaqueIsvKey) =>
            Message().Write(RequestCode.VerifyAndAuthorizeRequest, "1", apiName, apiVersion, opaqueIsvKey).Send();

        // For IB's internal purpose. Allows to provide means of verification between the TWS and third party programs.
        public void VerifyAndAuthorizeMessage(string apiData, string xyzResponse) =>
            Message().Write(RequestCode.VerifyAndAuthorizeMessage, "1", apiData, xyzResponse).Send();

        /**
        * Requests position subscription for account and/or model
        * Initially all positions are returned, and then updates are returned for any position changes in real time.
        * @param account - If an account Id is provided, only the account's positions belonging to the specified model will be delivered
        * @param modelCode - The code of the model's positions we are interested in.
        * @sa cancelPositionsMulti, EWrapper::positionMulti, EWrapper::positionMultiEnd
        */
        public void RequestPositionsMulti(int requestId, string account, string modelCode)
        {
            Config.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
            Message().Write(RequestCode.RequestPositionsMulti, "1", requestId, account, modelCode).Send();
        }

        public void CancelPositionsMulti(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
            Message().Write(RequestCode.CancelPositionsMulti, "1", requestId).Send();
        }

        public void RequestAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNlv)
        {
            Config.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
            Message().Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId, account, modelCode, ledgerAndNlv).Send();
        }

        public void CancelAccountUpdatesMulti(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.MODELS_SUPPORT);
            Message().Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId).Send();
        }

        public void RequestSecDefOptParams(int requestId, string underlyingSymbol, string futFopExchange, string underlyingSecType, int underlyingConId)
        {
            Config.RequireServerVersion(ServerVersion.SEC_DEF_OPT_PARAMS_REQ);
            Message()
                .Write(RequestCode.RequestSecurityDefinitionOptionalParameters, requestId,
                    underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId)
                .Send();
        }

        public void RequestSoftDollarTiers(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.SOFT_DOLLAR_TIER);
            Message()
                .Write(RequestCode.RequestSoftDollarTiers, requestId)
                .Send();
        }

        public void RequestFamilyCodes()
        {
            Config.RequireServerVersion(ServerVersion.REQ_FAMILY_CODES);
            Message().Write(RequestCode.RequestFamilyCodes).Send();
        }

        public void RequestMatchingSymbols(int requestId, string pattern)
        {
            Config.RequireServerVersion(ServerVersion.REQ_MATCHING_SYMBOLS);
            Message().Write(RequestCode.RequestMatchingSymbols, requestId, pattern).Send();
        }

        // venues for which market data is returned to updateMktDepthL2(those with market makers)
        public void RequestMarketDepthExchanges()
        {
            Config.RequireServerVersion(ServerVersion.REQ_MKT_DEPTH_EXCHANGES);
            Message().Write(RequestCode.RequestMktDepthExchanges).Send();
        }

        // Returns the mapping of single letter codes to exchange names given the mapping identifier
        public void RequestSmartComponents(int requestId, string bboExchange)
        {
            Config.RequireServerVersion(ServerVersion.REQ_MKT_DEPTH_EXCHANGES);
            Message().Write(RequestCode.RequestSmartComponents, requestId, bboExchange).Send();
        }

        public void RequestNewsArticle(int requestId, string providerCode, string articleId)
        {
            Config.RequireServerVersion(ServerVersion.REQ_NEWS_ARTICLE);
            Message().Write(RequestCode.RequestNewsArticle, requestId, providerCode, articleId).Send();
        }

        public void RequestNewsProviders()
        {
            Config.RequireServerVersion(ServerVersion.REQ_NEWS_PROVIDERS);
            Message().Write(RequestCode.RequestNewsProviders).Send();
        }

        public void RequestHistoricalNews(int requestId, int conId, string providerCodes, string startTime, string endTime, int totalResults)
        {
            Config.RequireServerVersion(ServerVersion.REQ_HISTORICAL_NEWS);
            Message()
                .Write(RequestCode.RequestHistoricalNews, requestId, conId, providerCodes, startTime, endTime, totalResults)
                .Send();
        }

        // Returns the timestamp of earliest available historical data for a contract and data type
        //* @param whatToShow - type of data for head timestamp - "BID", "ASK", "TRADES", etc
        //* @param useRTH - use regular trading hours only, 1 for yes or 0 for no
        //* @param formatDate - @param formatDate set to 1 to obtain the bars' time as yyyyMMdd HH:mm:ss, set to 2 to obtain it like system time format in seconds
        public void RequestHeadTimestamp(int requestId, Contract contract, string whatToShow, int useRth, int formatDate)
        {
            Config.RequireServerVersion(ServerVersion.REQ_HEAD_TIMESTAMP);
            Message()
                .Write(RequestCode.RequestHeadTimestamp, requestId)
                .WriteContract(contract)
                .Write(useRth, whatToShow, formatDate)
                .Send();
        }

        // Returns data histogram of specified contract
        public void RequestHistogramData(int requestId, Contract contract, bool useRth, string period)
        {
            Config.RequireServerVersion(ServerVersion.REQ_HISTOGRAM_DATA);
            Message()
                .Write(RequestCode.RequestHistogramData, requestId)
                .WriteContract(contract)
                .Write(useRth, period)
                .Send();
        }

        public void CancelHistogramData(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.REQ_HISTOGRAM_DATA);
            Message().Write(RequestCode.CancelHistogramData, requestId).Send();
        }
    }
}
