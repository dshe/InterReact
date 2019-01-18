using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Extensions;
using NodaTime;
using NodaTime.Text;

namespace InterReact.Core
{
    /// <summary>
    /// Methods which send request messages to TWS/Gateway.
    /// Request methods are serialized, thread-safe and limited to a specified number of messages per second.
    /// </summary>
    public sealed class Request : IEditorBrowsableNever
    {
        private readonly Config Config;
        internal Func<RequestMessage> CreateMessage;

        internal Request(in Config config, Func<RequestMessage> createMessage)
        {
            Config = config;
            CreateMessage = createMessage;
        }

        /// <summary>
        /// Returns successive unique ids which are used to uniquely identify requests and orders.
        /// </summary>
        public int NextId() => Interlocked.Increment(ref Config.NextIdValue);

        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Call this method to request market data. The market data will be returned as Tick* objects.
        /// Include the generic Tick RealtimeVolume to receive TickRealTimeVolume ticks.
        /// You can turn off regular market data so that only generic tick data is returned by also including the generic tick "mdoff".
        /// </summary>
        public void RequestMarketData(int requestId, Contract contract,
            IEnumerable<GenericTickType> genericTickTypes = null, bool marketDataOff = false,
            bool isSnapshot = false, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            var m = CreateMessage();

            m.Write(RequestCode.RequestMarketData, "11", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass);

            if (contract.SecurityType == SecurityType.Bag)
            {
                m.Write(contract.ComboLegs.Count);
                foreach (var leg in contract.ComboLegs)
                    m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange);
            }
            m.Write(contract.Undercomp != null);
            if (contract.Undercomp != null)
                m.Write(contract.Undercomp.ContractId, contract.Undercomp.Delta, contract.Undercomp.Price);
            m.Write(MakeGenericTicksList(genericTickTypes, marketDataOff), isSnapshot);
            m.WriteTagsAsOneString(options);

            m.Send();
        }

        private static string MakeGenericTicksList(IEnumerable<GenericTickType> genericTickTypes, bool marketDataOff = false)
        {
            var strings = new List<string>();
            if (genericTickTypes != null && genericTickTypes.Any())
                strings.AddRange(genericTickTypes.Cast<Enum>().Select(Convert.ToInt32).Select(n => n.ToString()));
            if (marketDataOff)
                strings.Add("mdoff");
            return strings.JoinStrings(",");
        }

        public void CancelMarketData(int requestId) => 
            CreateMessage().Write(RequestCode.CancelMarketData, "1", requestId).Send();

        public void PlaceOrder(int orderId, Order order, Contract contract)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (!string.IsNullOrEmpty(order.ExtOperator))
                Config.RequireServerVersion(ServerVersion.ExtOperator);
            if (order.CashQty != null)
                Config.RequireServerVersion(ServerVersion.CashQty);

            var m = CreateMessage();

            m.Write(RequestCode.PlaceOrder, "45", orderId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange, contract.PrimaryExchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass,
                contract.SecurityIdType, contract.SecurityId, order.TradeAction);

            if (Config.SupportsServerVersion(ServerVersion.FractionalPositions))
                m.Write(order.TotalQuantity);
            else
                m.Write((int) order.TotalQuantity);

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
                foreach (var leg in contract.ComboLegs)
                    m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange,
                        leg.OpenClose, leg.ComboShortSaleSlot, leg.DesignatedLocation, leg.ExemptCode);
                m.Write(order.ComboLegs.Count);
                foreach (var leg in order.ComboLegs)
                    m.Write(leg.Price);
                m.Write(order.SmartComboRoutingParams.Count);
                foreach (var tag in order.SmartComboRoutingParams)
                    m.Write(tag.Name, tag.Value);
            }
            m.Write("", // deprecated SharesAllocation field
                order.DiscretionaryAmount,
                order.GoodAfterTime, order.GoodUntilDate,
                order.FinancialAdvisorGroup, order.FinancialAdvisorMethod, order.FinancialAdvisorPercentage,
                order.FinancialAdvisorProfile);

            if (Config.SupportsServerVersion(ServerVersion.ModelsSupport))
                m.Write(order.ModelCode);

            m.Write(order.ShortSaleSlot, order.DesignatedLocation, order.ExemptCode,
                order.OcaType, order.Rule80A,
                order.SettlingFirm, order.AllOrNone,
                order.MinimumQuantity, order.PercentOffset,
                order.ElectronicTradeOnly, order.FirmQuoteOnly,
                order.NbboPriceCap, order.AuctionStrategy,
                order.StartingPrice, order.StockReferencePrice,
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
            m.Write(contract.Undercomp != null);

            if (contract.Undercomp != null)
                m.Write(contract.Undercomp.ContractId, contract.Undercomp.Delta,
                    contract.Undercomp.Price);

            m.Write(order.AlgoStrategy);

            if (!string.IsNullOrEmpty(order.AlgoStrategy))
            {
                m.Write(order.AlgoParams.Count);
                foreach (var tag in order.AlgoParams)
                    m.Write(tag.Name, tag.Value);
            }

            m.Write(order.AlgoId, order.WhatIf)
                .WriteTagsAsOneString(order.MiscOptions)
                .Write(order.Solicited);

            m.Write(order.RandomizeSize, order.RandomizePrice);

            if (Config.SupportsServerVersion(ServerVersion.PeggedToBenchmark))
            {
                if (order.OrderType == OrderType.PeggedToBenchmark)
                    m.Write(order.ReferenceContractId, order.IsPeggedChangeAmountDecrease,
                        order.PeggedChangeAmount, order.ReferenceChangeAmount, order.ReferenceExchange);
                var n = order.Conditions.Count;
                m.Write(n);
                if (n > 0)
                {
                    foreach (var condition in order.Conditions)
                        condition.Serialize(m);
                    m.Write(order.ConditionsIgnoreRegularTradingHours, order.ConditionsCancelOrder);
                }
                m.Write(order.AdjustedOrderType, order.TriggerPrice, order.LmtPriceOffset, order.AdjustedStopPrice,
                    order.AdjustedStopLimitPrice, order.AdjustedTrailingAmount, order.AdjustableTrailingUnit);
            }

            if (Config.SupportsServerVersion(ServerVersion.ExtOperator))
                m.Write(order.ExtOperator);

            if (Config.SupportsServerVersion(ServerVersion.SoftDollarTier))
                m.Write(order.SoftDollarTier.Name, order.SoftDollarTier.Value);

            if (Config.SupportsServerVersion(ServerVersion.CashQty))
                m.Write(order.CashQty);

            m.Send();
        }

        public void CancelOrder(int orderId) => CreateMessage().Write(RequestCode.CancelOrder, "1", orderId).Send();

        public void RequestOpenOrders() => CreateMessage().Write(RequestCode.RequestOpenOrders, "1").Send();

        /// <summary>
        /// Call this function to start receiving account updates.
        /// Returns AccountVaue, PortfolioUpdate and TimeUpdate messages.
        /// Updates for all accounts are returned when accountCode is null.
        /// This information is updated every three minutes.
        /// </summary>
        public void RequestAccountUpdates(bool start, string accountCode = null) =>
            CreateMessage().Write(RequestCode.RequestAccountData, "2", start, accountCode).Send();

        /// <summary>
        /// When this method is called, execution reports from the last 24 hours that meet the filter criteria are retrieved.
        /// To view executions beyond the past 24 hours, open the trade log in TWS and, while the Trade Log is displayed, select the days to be returned.
        /// When ExecutionFilter is null, all executions are returned.
        /// </summary>
        public void RequestExecutions(int requestId, ExecutionFilter filter = null) =>
            CreateMessage().Write(RequestCode.RequestExecutions, "3", requestId,
                filter?.ClientId, filter?.Account,
                filter == null ? null : LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd-HH:mm:ss").Format(filter.Time),
                filter?.Symbol, filter?.SecurityType,
                filter?.Exchange, filter?.Side).Send();

        /// <summary>
        /// In case there are multiple API clients attached to TWS, the OrderId may not be unique among the clients.
        /// Call this function to request an OrderId that can be used with multiple clients.
        /// </summary>
        public void RequestNextOrderId() => CreateMessage().Write(RequestCode.RequestIds, "1", "1").Send();

        /// <summary>
        /// Call this method to retrieve one or more ContractDetails objects for the specified selector contract.
        /// </summary>
        public void RequestContractDetails(int requestId, Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            CreateMessage().Write(RequestCode.RequestContractDetails, "8", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier,
                contract.Exchange, contract.PrimaryExchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass,
                contract.IncludeExpired, contract.SecurityIdType, contract.SecurityId).Send();
        }

        /// <summary>
        /// Call this method to request market depth for the specified contract. 
        /// </summary>
        public void RequestMarketDepth(int requestId, Contract contract, int numRows = 3, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            CreateMessage().Write(RequestCode.RequestMarketDepth, "5", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass, numRows)
                .WriteTagsAsOneString(options).Send();
        }

        public void CancelMarketDepth(int requestId) => CreateMessage().Write(RequestCode.CancelMarketDepth, "1", requestId).Send();

        /// <summary>
        /// Call this method to start receiving news bulletins, such as information about exchange status.
        /// </summary>
        public void RequestNewsBulletins(bool all = true) => CreateMessage().Write(RequestCode.RequestNewsBulletins, "1", all).Send();

        public void CancelNewsBulletins() => CreateMessage().Write(RequestCode.CancelNewsBulletins, "1").Send();

        public void SetServerLogLevel(LogEntryLevel logLevel)
            => CreateMessage().Write(RequestCode.SetServerLogLevel, "1", logLevel).Send();

        public void RequestAutoOpenOrders(bool autoBind) => CreateMessage().Write(RequestCode.RequestAutoOpenOrders, "1", autoBind).Send();

        public void RequestAllOpenOrders() => CreateMessage().Write(RequestCode.RequestAllOpenOrders, "1").Send();

        public void RequestManagedAccounts() => CreateMessage().Write(RequestCode.RequestManagedAccounts, "1").Send();

        public void RequestFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType) =>
            CreateMessage().Write(RequestCode.RequestFinancialAdvisorConfiguration, "1", dataType).Send();

        public void ReplaceFinancialAdvisorConfiguration(FinancialAdvisorDataType dataType, string xml) =>
            CreateMessage().Write(RequestCode.ReplaceFinancialAdvisorConfiguration, "1", dataType, xml).Send();

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
            HistoricalDuration duration = null,
            HistoricalBarSize barSize = null,
            HistoricalDataType dataType = null,
            bool regularTradingHoursOnly = false,
            params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            if (endDate == default)
                endDate = Config.Clock.GetCurrentInstant();

            var m = CreateMessage();

            m.Write(RequestCode.RequestHistoricalData, "6", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType,
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
                foreach (var leg in contract.ComboLegs)
                    m.Write(leg.ContractId, leg.Ratio, leg.TradeAction, leg.Exchange);
            }
            m.WriteTagsAsOneString(options);

            m.Send();
        }

        public void ExerciseOptions(int requestId, Contract contract, OptionExerciseAction exerciseAction, int exerciseQuantity,
            string account, bool overrideOrder)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            CreateMessage().Write(RequestCode.ExerciseOptions, "2", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.Currency, contract.LocalSymbol, contract.TradingClass,
                exerciseAction, exerciseQuantity, account, overrideOrder).Send();
        }

        /// <summary>
        /// Call this method to start receiving market scanner results.
        /// </summary>
        public void RequestScannerSubscription(int requestId, ScannerSubscription subscription, params Tag[] options)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
            CreateMessage().Write(RequestCode.RequestScannerSubscription, "4", requestId,
                subscription.NumberOfRows, subscription.Instrument,
                subscription.LocationCode, subscription.ScanCode,
                subscription.AbovePrice, subscription.BelowPrice, subscription.AboveVolume,
                subscription.MarketCapAbove, subscription.MarketCapBelow,
                subscription.MoodyRatingAbove, subscription.MoodyRatingBelow,
                subscription.SpRatingAbove, subscription.SpRatingBelow,
                subscription.MaturityDateAbove, subscription.MaturityDateBelow,
                subscription.CouponRateAbove, subscription.CouponRateBelow,
                subscription.ExcludeConvertible, subscription.AverageOptionVolumeAbove,
                subscription.ScannerSettingPairs, subscription.StockType)
                .WriteTagsAsOneString(options).Send();
        }

        public void CancelScannerSubscription(int requestId) =>
            CreateMessage().Write(RequestCode.CancelScannerSubscription, "1", requestId).Send();

        /// <summary>
        /// Call the this method to receive an XML document that describes the valid parameters that a scanner subscription can have.
        /// </summary>ca
        public void RequestScannerParameters() =>
            CreateMessage().Write(RequestCode.RequestScannerParameters, "1").Send();

        public void CancelHistoricalData(int requestId) =>
            CreateMessage().Write(RequestCode.CancelHistoricalData, "1", requestId).Send();

        /// <summary>
        /// This is the time reported by TWS. Seconds precision.
        /// </summary>
        public void RequestCurrentTime() => CreateMessage().Write(RequestCode.RequestCurrentTime, 1).Send();

        /// <summary>
        /// Call this method to start receiving realtime bar data.
        /// </summary>
        public void RequestRealTimeBars(int requestId, Contract contract, RealtimeBarType whatToShow = null,
            bool regularTradingHoursOnly = false, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            CreateMessage().Write(RequestCode.RequestRealtimeBars, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                "5", // only 5 seconds bars are available
                whatToShow ?? RealtimeBarType.Trades, 
                regularTradingHoursOnly)
                .WriteTagsAsOneString(options).Send();
        }

        public void CancelRealTimeBars(int requestId) => CreateMessage().Write(RequestCode.CancelRealtimeBars, "1", requestId).Send();

        /// <summary>
        /// Call this method to receive Reuters global fundamental data for stocks.
        /// There must be a subscription to Reuters Fundamental set up in Account Management before you can receive this data.
        /// IB: The method can handle conid specified in the Contract object, but not tradingClass or multiplier.
        /// </summary>
        public void RequestFundamentalData(int requestId, Contract contract, FundamentalDataReportType reportType = null, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            reportType = reportType ?? FundamentalDataReportType.CompanyOverview;
            CreateMessage().Write(RequestCode.RequestFundamentalData, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol,
                reportType.ToString()).WriteTagsAsOneString(options).Send();
        }

        public void CancelFundamentalData(int requestId) => CreateMessage().Write(RequestCode.CancelFundamentalData, "1", requestId).Send();

        /// <summary>
        /// Call this function to calculate volatility for a supplied option price and underlying price.
        /// </summary>
        public void CalculateImpliedVolatility(int requestId, Contract contract, double optionPrice, double underlyingPrice, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            CreateMessage().Write(RequestCode.RequestCalculatedImpliedVolatility, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                optionPrice, underlyingPrice).WriteTagsAsOneString(options).Send();
        }

        /// <summary>
        /// Call this function to calculate option price and greek Values for a supplied volatility and underlying price.
        /// </summary>
        public void CalculateOptionPrice(int requestId, Contract contract, double volatility, double underlyingPrice, params Tag[] options)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            CreateMessage().Write(RequestCode.RequestCalculatedOptionPrice, "3", requestId,
                contract.ContractId, contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                contract.Strike, contract.Right, contract.Multiplier, contract.Exchange,
                contract.PrimaryExchange, contract.Currency, contract.LocalSymbol, contract.TradingClass,
                volatility, underlyingPrice).WriteTagsAsOneString(options).Send();
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
        public void RequestMarketDataType(MarketDataType marketDataType)
            => CreateMessage().Write(RequestCode.RequestMarketDataType, "1", marketDataType).Send();

        /// <summary>
        /// Requests real-time position data for all accounts.
        /// </summary>
        public void RequestAccountPositions() => CreateMessage().Write(RequestCode.RequestAccountPositions, "1").Send();


        /// <summary>
        /// Call this method to request the data that appears on the TWS Account Window Summary tab. 
        /// If no tags are specified, data for all tags will be requested.
        /// </summary>
        public void RequestAccountSummary(int requestId, string group = "All", params AccountSummaryTag[] tags)
        {
            var tagNames = tags?.Select(tag => tag.ToString()).ToList();
            if (tagNames == null || tagNames.Count == 0)
                tagNames = Enum.GetNames(typeof(AccountSummaryTag)).ToList();
            CreateMessage().Write(RequestCode.RequestAccountSummary, "1", requestId, group, string.Join(",", tagNames)).Send();
        }

        public void CancelAccountSummary(int requestId) =>
            CreateMessage().Write(RequestCode.CancelAccountSummary, "1", requestId).Send();

        public void CancelAccountPositions() =>
            CreateMessage().Write(RequestCode.CancelAccountPositions, "1").Send();

        public void VerifyRequest(string apiName, string apiVersion) =>
            CreateMessage().Write(RequestCode.VerifyRequest, "1", apiName, apiVersion).Send();

        public void VerifyMessage(string apiData) =>
            CreateMessage().Write(RequestCode.VerifyMessage, "1", apiData).Send();

        public void QueryDisplayGroups(int requestId) =>
            CreateMessage().Write(RequestCode.QueryDisplayGroups, "1", requestId).Send();

        public void SubscribeToGroupEvents(int requestId, int groupId) =>
            CreateMessage().Write(RequestCode.SubscribeToGroupEvents, "1", requestId, groupId).Send();

        public void UpdateDisplayGroup(int requestId, string contractInfo) =>
            CreateMessage().Write(RequestCode.UpdateDisplayGroup, "1", requestId, contractInfo).Send();

        public void UnsubscribeFromGroupEvents(int requestId) =>
            CreateMessage().Write(RequestCode.UnsubscribeFromGroupEvents, "1", requestId).Send();

        public void VerifyAndAuthorizeRequest(string apiName, string apiVersion, string opaqueIsvKey) =>
            CreateMessage().Write(RequestCode.VerifyAndAuthorizeRequest, "1", apiName, apiVersion, opaqueIsvKey).Send();

        public void VerifyAndAuthorizeMessage(string apiData, string xyzResponse) =>
            CreateMessage().Write(RequestCode.VerifyAndAuthorizeMessage, "1", apiData, xyzResponse).Send();

        public void RequestPositionsMulti(int requestId, string account, string modelCode)
        {
            Config.RequireServerVersion(ServerVersion.ModelsSupport);
            CreateMessage().Write(RequestCode.RequestPositionsMulti, "1", requestId, account, modelCode).Send();
        }

        public void CancelPositionsMulti(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.ModelsSupport);
            CreateMessage().Write(RequestCode.CancelPositionsMulti, "1", requestId).Send();
        }

        public void RequestAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNlv)
        {
            Config.RequireServerVersion(ServerVersion.ModelsSupport);
            CreateMessage().Write(RequestCode.RequestAccountUpdatesMulti, "1", requestId, account, modelCode, ledgerAndNlv).Send();
        }

        public void CancelAccountUpdatesMulti(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.ModelsSupport);
            CreateMessage().Write(RequestCode.CancelAccountUpdatesMulti, "1", requestId).Send();
        }

        public void RequestSecDefOptParams(int requestId, string underlyingSymbol, string futFopExchange, string underlyingSecType, int underlyingConId)
        {
            Config.RequireServerVersion(ServerVersion.SecurityDefinitionOptionalParametersRequest);
            CreateMessage().Write(RequestCode.RequestSecurityDefinitionOptionalParameters, requestId,
                underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId).Send(); // no version!
        }

        public void RequestSoftDollarTiers(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.SoftDollarTier);
            CreateMessage().Write(RequestCode.RequestSoftDollarTiers, requestId).Send(); // no version!
        }

        public void RequestFamilyCodes()
        {
            Config.RequireServerVersion(ServerVersion.RequestFamilyCodes);
            CreateMessage().Write(RequestCode.RequestFamilyCodes).Send();
        }

        public void RequestMatchingSymbols(int requestId, string pattern)
        {
            Config.RequireServerVersion(ServerVersion.RequestMatchingSymbols);
            CreateMessage().Write(RequestCode.RequestMatchingSymbols, requestId, pattern).Send();
        }

        public void RequestMarketDepthExchanges()
        {
            Config.RequireServerVersion(ServerVersion.RequestMktDepthExchanges);
            CreateMessage().Write(RequestCode.RequestMktDepthExchanges).Send();
        }

        public void RequestSmartComponents(int requestId, string bboExchange)
        {
            Config.RequireServerVersion(ServerVersion.RequestMktDepthExchanges);
            CreateMessage().Write(RequestCode.RequestSmartComponents, requestId, bboExchange).Send();
        }

        public void RequestNewsArticle(int requestId, string providerCode, string articleId)
        {
            Config.RequireServerVersion(ServerVersion.RequestNewsArticle);
            CreateMessage().Write(RequestCode.RequestNewsArticle, requestId, providerCode, articleId).Send();
        }

        public void RequestNewsProviders()
        {
            Config.RequireServerVersion(ServerVersion.RequestNewsProviders);
            CreateMessage().Write(RequestCode.RequestNewsProviders).Send();
        }

        public void RequestHistoricalNews(int requestId, int conId, string providerCodes, string startTime, string endTime, int totalResults)
        {
            Config.RequireServerVersion(ServerVersion.RequestHistoricalNews);
            CreateMessage()
                .Write(RequestCode.RequestHistoricalNews, requestId, conId, providerCodes, startTime, endTime, totalResults)
                .Send();
        }

        public void RequestHeadTimestamp(int requestId, Contract contract, string whatToShow, int useRth, int formatDate)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            Config.RequireServerVersion(ServerVersion.RequestHeadTimestamp);
            CreateMessage()
                .Write(RequestCode.RequestHeadTimestamp, requestId)
                .WriteContract(contract)
                .Write(useRth, whatToShow, formatDate)
                .Send();
        }

        public void RequestHistogramData(int requestId, Contract contract, bool useRth, string period)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            Config.RequireServerVersion(ServerVersion.RequestHistogramData);
            CreateMessage()
                .Write(RequestCode.RequestHistogramData, requestId)
                .WriteContract(contract)
                .Write(useRth, period)
                .Send();
        }

        public void CancelHistogramData(int requestId)
        {
            Config.RequireServerVersion(ServerVersion.RequestHistogramData);
            CreateMessage().Write(RequestCode.CancelHistogramData, requestId).Send();
        }
    }
}
