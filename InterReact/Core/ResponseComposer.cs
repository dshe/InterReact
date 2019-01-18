using System;
using System.Collections.Generic;
using System.Linq;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.Messages.Conditions;
using InterReact.Utility;
using InterReact.StringEnums;
using System.IO;
using System.Reactive.Linq;
using System.Globalization;
using NodaTime;
using InterReact.Extensions;

namespace InterReact.Core
{
    internal static class ToMessagesEx
    {
        internal static IObservable<object> ToMessages(this IObservable<string[]> source, Config config)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Observable.Create<object>(observer =>
            {
                var responseComposer = new ResponseComposer(config, observer.OnNext);

                return source.Subscribe(
                    onNext: responseComposer.Compose,
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted);
            });
        }
    }

    public sealed partial class ResponseComposer
    {
        private readonly Config config;
        private IEnumerator<string> enumerator;
        private Action<object> output;

        internal ResponseComposer(Config config, Action<object> output)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        internal void Compose(string[] inputStrings)
        {
            enumerator = inputStrings.AsEnumerable().GetEnumerator();
            try
            {
                Compose();
                if (enumerator.MoveNext())
                    output(new ResponseWarning($"Message longer than expected: [{inputStrings.JoinStrings(", ")}]."));
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"ComposeMessageError: [{inputStrings.JoinStrings(", ")}].", e);
            }
        }

        private void Compose()
        {
            var code = ReadString();
            if (code == "1")
                TickPrice();
            else
                output(Compose(code));
        }

        private object Compose(string code)
        {
            switch (code)
            {
                case "2":
                    return TickSize();
                case "3":
                    return OrderStatus();
                case "4":
                    return Alert();
                case "5":
                    return OpenOrder();
                case "6":
                    return AccountValue();
                case "7":
                    return PortfolioValue();
                case "8":
                    return AccountUpdateTime();
                case "9":
                    return NextId();
                case "10":
                    return ContractDetails();
                case "11":
                    return Execution();
                case "12":
                    return MarketDepth(false);
                case "13":
                    return MarketDepth(true);
                case "14":
                    return NewsBulletins();
                case "15":
                    return ManagedAccounts();
                case "16":
                    return FinancialAdvisor();
                case "17":
                    return HistoricalData();
                case "18":
                    return BondContractDetails();
                case "19":
                    return ScannerParameters();
                case "20":
                    return ScannerData();
                case "21":
                    return TickOptionComputation();

                case "45":
                    return TickGeneric();
                case "46":
                    return TickString();
                case "47":
                    return TickExchangeForPhysical();

                case "49":
                    return CurrentTime();
                case "50":
                    return RealtimeBar();
                case "51":
                    return FundamentalData();
                case "52":
                    return ContractDetailsEnd();
                case "53":
                    return OpenOrderEnd();
                case "54":
                    return AccountUpdateEnd();
                case "55":
                    return ExecutionEnd();
                case "56":
                    return DeltaNeutralValidation();
                case "57":
                    return TickSnapshotEnd();
                case "58":
                    return MarketDataType();
                case "59":
                    return CommissionReport();

                case "61":
                    return AccountPosition();
                case "62":
                    return AccountPositionEnd();
                case "63":
                    return AccountSummary();
                case "64":
                    return AccountSummaryEnd();
                case "65":
                    return VerifyMessageApi();
                case "66":
                    return VerifyCompleted();
                case "67":
                    return DisplayGroups();
                case "68":
                    return DisplayGroupUpdate();
                case "69":
                    return VerifyAndAuthorizeMessageApi();
                case "70":
                    return VerifyAndAuthorizeCompleted();
                case "71":
                    return PositionMulti();
                case "72":
                    return PositionMultiEnd();
                case "73":
                    return AccountUpdateMulti();
                case "74":
                    return AccountUpdateMultiEnd();
                case "75":
                    return SecurityDefinitionOptionParameter();
                case "76":
                    return SecurityDefinitionOptionParameterEnd();
                case "77":
                    return SoftDollarTiers();
                case "78":
                    return FamilyCodes();
                case "79":
                    return SymbolSamples();
                case "80":
                    return MarketDepthExchanges();
                case "81":
                    return TickReqParams();
                case "82":
                    return SmartComponents();
                case "83":
                    return NewsArticle();
                case "84":
                    return TickNews();
                case "85":
                    return NewsProviders();
                case "86":
                    return HistoricalNews();
                case "87":
                    return HistoricalNewsEnd();
                case "88":
                    return HeadTimestamp();
                case "89":
                    return HistogramData();

                default:
                    throw new InvalidDataException($"Undefined code '{code}'.");
            }
        }

        // This is the only method that can compose more than message. Output(message) used instead of return(message).
        private void TickPrice()
        {
            var version = GetVersion();
            var requestId = Read<int>();
            if (requestId == int.MaxValue) // used to test failure
                Read<double>(); // trigger parse exception
            var priceTickType = Read<TickType>();
            var price = Read<double>();
            var size = version >= 2 ? Read<int>() : 0;

            var tickPrice = new TickPrice
            {
                RequestId = requestId,
                TickType = priceTickType,
                Price = price
            };

            if (version >= 3)
            {
                var i = Read<int>();
                tickPrice.CanAutoExecute = i == 1;
                if (config.SupportsServerVersion(ServerVersion.PastLimit))
                {
                    var mask = new BitMask(i);
                    tickPrice.CanAutoExecute = mask[0];
                    tickPrice.PastLimit = mask[1];
                }
            }

            output(tickPrice);

            if (version >= 2)
            {
                (bool success, TickType tickTypeSize) = TickTypeSize.GetTickTypeSize(priceTickType);
                if (success)
                    output(new Tick[] { tickPrice, new TickSize { RequestId = requestId, TickType = tickTypeSize, Size = size } });
            }
        }

        private TickSize TickSize()
        {
            IgnoreVersion();
            return new TickSize
            {
                RequestId = Read<int>(),
                TickType = Read<TickType>(),
                Size = Read<int>()
            };
        }

        private Tick TickGeneric()
        {
            IgnoreVersion();
            var requestId = Read<int>();
            var tickType = Read<TickType>();
            var value = Read<double>();
            if (tickType == TickType.Halted)
            {
                return new TickHalted
                {
                    RequestId = requestId,
                    TickType = tickType,
                    HaltType = value == 0 ? HaltType.NotHalted : HaltType.GeneralHalt
                };
            }
            return new TickGeneric
            {
                RequestId = requestId,
                TickType = tickType,
                Value = value
            };
        }

        private Tick TickString()
        {
            IgnoreVersion();
            var requestId = Read<int>();
            var tickType = Read<TickType>();
            var str = ReadString();

            if (tickType == TickType.RealtimeVolume)
            {
                var parts = str.Split(';');
                return new TickRealtimeVolume
                {
                    RequestId = requestId,
                    TickType = TickType.RealtimeVolume,
                    Price = Parse<double>(parts[0]),
                    Size = Parse<int>(parts[1]),
                    Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo)),
                    Volume = Parse<int>(parts[3]),
                    Vwap = Parse<double>(parts[4]),
                    SingleTrade = Parse<bool>(parts[5])
                };
            }
            if (tickType == TickType.LastTimeStamp)
            {
                return new TickTime
                {
                    RequestId = requestId,
                    TickType = TickType.LastTimeStamp,
                    Time = Instant.FromUnixTimeSeconds(long.Parse(str, NumberFormatInfo.InvariantInfo))
                };
            }
            return new TickString
            {
                RequestId = requestId,
                TickType = tickType,
                Value = str
            };
        }

        private TickOptionComputation TickOptionComputation()
        {
            var version = GetVersion();
            var t = new TickOptionComputation
            {
                RequestId = Read<int>(),
                TickType = Read<TickType>(),
                ImpliedVolatility = Read<double?>()
            };
            if (t.ImpliedVolatility < 0)
                t.ImpliedVolatility = null;

            t.Delta = Read<double?>();
            if (t.Delta != null && Math.Abs(t.Delta.Value) > 1)
                t.Delta = null;

            if (version >= 6 || t.TickType == TickType.ModelOptionComputation)
            {
                t.OptPrice = Read<double?>();
                if (t.OptPrice < 0)
                    t.OptPrice = null;
                t.PvDividend = Read<double?>();
                if (t.PvDividend < 0)
                    t.PvDividend = null;
            }
            if (version >= 6)
            {
                t.Gamma = Read<double?>();
                if (t.Gamma != null && Math.Abs(t.Gamma.Value) > 1)
                    t.Gamma = null;
                t.Vega = Read<double?>();
                if (t.Vega != null && Math.Abs(t.Vega.Value) > 1)
                    t.Vega = null;
                t.Theta = Read<double?>();
                if (t.Theta != null && Math.Abs(t.Theta.Value) > 1)
                    t.Theta = null;
                t.UndPrice = Read<double?>();
                if (t.UndPrice < 0)
                    t.UndPrice = null;
            }
            return t;
        }

        private TickExchangeForPhysical TickExchangeForPhysical()
        {
            IgnoreVersion();
            return new TickExchangeForPhysical
            {
                RequestId = Read<int>(),
                TickType = Read<TickType>(),
                BasisPoints = Read<double>(),
                FormattedBasisPoints = ReadString(),
                ImpliedFuturesPrice = Read<double>(),
                HoldDays = Read<int>(),
                FutureExpiry = ReadString(),
                DividendImpact = Read<double>(),
                DividendsToExpiry = Read<double>()
            };
        }

        private TickMarketDataType MarketDataType()
        {
            IgnoreVersion();
            return new TickMarketDataType
            {
                RequestId = Read<int>(),
                TickType = TickType.MarketDataType,
                MarketDataType = Read<MarketDataType>()
            };
        }

        private TickSnapshotEnd TickSnapshotEnd()
        {
            IgnoreVersion();
            return new TickSnapshotEnd { RequestId = Read<int>() };
        }

        private ManagedAccounts ManagedAccounts()
        {
            IgnoreVersion();
            return new ManagedAccounts
            {
                Accounts = ReadString().Split(',').OrderBy(name => name).ToList()
            };
        }

        private AccountValue AccountValue()
        {
            RequireVersion(2);
            return new AccountValue
            {
                Key = ReadString(),
                Value = ReadString(),
                Currency = ReadString(),
                Account = ReadString()
            };
        }

        private AccountUpdateTime AccountUpdateTime()
        {
            IgnoreVersion();
            return new AccountUpdateTime
            {
                Time = Read<LocalTime>()
            };
        }

        private AccountUpdateEnd AccountUpdateEnd()
        {
            IgnoreVersion();
            return new AccountUpdateEnd { Account = ReadString() };
        }

        private AccountPositionEnd AccountPositionEnd()
        {
            IgnoreVersion();
            return new AccountPositionEnd();
        }

        private AccountSummary AccountSummary()
        {
            IgnoreVersion();
            return new AccountSummary
            {
                RequestId = Read<int>(),
                Account = ReadString(),
                Tag = ReadString(),
                Value = ReadString(),
                Currency = ReadString()
            };
        }

        private AccountSummaryEnd AccountSummaryEnd()
        {
            IgnoreVersion();
            return new AccountSummaryEnd { RequestId = Read<int>() };
        }

        private OrderStatusReport OrderStatus()
        {
            var version = RequireVersion(5);
            return new OrderStatusReport
            {
                OrderId = Read<int>(),
                Status = ReadStringEnum<OrderStatus>(),
                Filled = Read<double>(), // int -> double
                Remaining = Read<double>(),
                AverageFillPrice = Read<double>(),
                PermanentId = Read<int>(),
                ParentId = Read<int>(),
                LastFillPrice = Read<double>(),
                ClientId = Read<int>(),
                WhyHeld = version >= 6 ? ReadString() : string.Empty
            };
        }

        private OpenOrderEnd OpenOrderEnd()
        {
            IgnoreVersion();
            return new OpenOrderEnd();
        }

        private ExecutionEnd ExecutionEnd()
        {
            IgnoreVersion();
            return new ExecutionEnd { RequestId = Read<int>() };
        }

        private CommissionReport CommissionReport()
        {
            IgnoreVersion();
            var executionId = ReadString();
            return new CommissionReport
            {
                ExecutionId = executionId,
                Execution = executions[executionId],
                Commission = Read<double>(),
                Currency = ReadString(),
                RealizedPnl = Read<double>(),
                Yield = Read<double>(),
                YieldRedemptionDate = Read<int>()
            };
        }

        private UnderComp DeltaNeutralValidation()
        {
            IgnoreVersion();
            return new UnderComp
            {
                RequestId = Read<int>(),
                ContractId = Read<int>(),
                Delta = Read<double>(),
                Price = Read<double>()
            };
        }

        private FinancialAdvisor FinancialAdvisor()
        {
            IgnoreVersion();
            return new FinancialAdvisor
            {
                DataType = Read<FinancialAdvisorDataType>(),
                XmlData = ReadString()
            };
        }

        private FundamentalData FundamentalData()
        {
            IgnoreVersion();
            return new FundamentalData
            {
                RequestId = Read<int>(),
                XmlData = ReadString()
            };
        }

        private NewsBulletin NewsBulletins()
        {
            IgnoreVersion();
            return new NewsBulletin
            {
                MessageId = Read<int>(),
                Type = Read<NewsBulletinType>(),
                Message = ReadString(),
                Origin = ReadString()
            };
        }

        private RealtimeBar RealtimeBar()
        {
            IgnoreVersion();
            return new RealtimeBar
            {
                RequestId = Read<int>(),
                Time = Instant.FromUnixTimeSeconds(long.Parse(ReadString(), NumberFormatInfo.InvariantInfo)),
                Open = Read<double>(),
                High = Read<double>(),
                Low = Read<double>(),
                Close = Read<double>(),
                Volume = Read<long>(),
                Wap = Read<double>(),
                Count = Read<int>()
            };
        }

        private ScannerParameters ScannerParameters()
        {
            IgnoreVersion();
            return new ScannerParameters { Parameters = ReadString() };
        }

        private ContractDetailsEnd ContractDetailsEnd()
        {
            IgnoreVersion();
            return new ContractDetailsEnd { RequestId = Read<int>() };
        }

        private CurrentTime CurrentTime()
        {
            IgnoreVersion();
            return new CurrentTime { Time = Instant.FromUnixTimeSeconds(long.Parse(ReadString(), NumberFormatInfo.InvariantInfo)) };
        }

        private NextId NextId()
        {
            IgnoreVersion();
            return new NextId { Id = Read<int>() };
        }

        private Alert Alert()
        {
            RequireVersion(2);
            return new Alert(id: Read<int>(), code: Read<int>(), message: ReadString());
        }

        private VerifyMessageApi VerifyMessageApi()
        {
            IgnoreVersion();
            return new VerifyMessageApi { Data = ReadString() };
        }

        private VerifyCompleted VerifyCompleted()
        {
            IgnoreVersion();
            return new VerifyCompleted
            {
                IsSuccessful = Read<bool>(),
                ErrorText = ReadString()
            };
        }

        private DisplayGroups DisplayGroups()
        {
            IgnoreVersion();
            return new DisplayGroups
            {
                RequestId = Read<int>(),
                Groups = ReadString()
            };
        }

        private DisplayGroupUpdate DisplayGroupUpdate()
        {
            IgnoreVersion();
            return new DisplayGroupUpdate
            {
                RequestId = Read<int>(),
                ContractInfo = ReadString()
            };
        }

        private VerifyAndAuthorizeCompleted VerifyAndAuthorizeCompleted()
        {
            IgnoreVersion();
            return new VerifyAndAuthorizeCompleted
            {
                IsSuccessful = Read<bool>(),
                ErrorText = ReadString()
            };
        }

        private VerifyAndAuthorizeMessageApi VerifyAndAuthorizeMessageApi()
        {
            IgnoreVersion();
            return new VerifyAndAuthorizeMessageApi
            {
                ApiData = ReadString(),
                XyzChallenge = ReadString()
            };
        }

        private MarketDepth MarketDepth(bool isLevel2)
        {
            IgnoreVersion();
            return new MarketDepth
            {
                RequestId = Read<int>(),
                Position = Read<int>(),
                MarketMaker = isLevel2 ? ReadString() : string.Empty,
                Operation = Read<MarketDepthOperation>(),
                Side = Read<MarketDepthSide>(),
                Price = Read<double>(),
                Size = Read<int>()
            };
        }

        private PortfolioValue PortfolioValue()
        {
            RequireVersion(8);
            return new PortfolioValue
            {
                Contract =
                {
                    ContractId = Read<int>(),
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Multiplier = ReadString(),
                    PrimaryExchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString(),
                    TradingClass = ReadString()
                },
                Position = Read<double>(),
                MarketPrice = Read<double>(),
                MarketValue = Read<double>(),
                AverageCost = Read<double>(),
                UnrealizedPnl = Read<double>(),
                RealizedPnl = Read<double>(),
                Account = ReadString()
            };
        }

        private AccountPosition AccountPosition()
        {
            RequireVersion(3);
            return new AccountPosition
            {
                Account = ReadString(),
                Contract =
                {
                    ContractId = Read<int>(),
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Multiplier = ReadString(),
                    Exchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString(),
                    TradingClass = ReadString()
                },
                Position = Read<double>(), // may be an int
                AverageCost = Read<double>()
            };
        }

        private OpenOrder OpenOrder() // the monster
        {
            var version = RequireVersion(17);
            var orderId = Read<int>();

            var openOrder = new OpenOrder
            {
                Contract =
                {
                    ContractId = Read<int>(),
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Multiplier = version >= 32 ? ReadString() : string.Empty,
                    Exchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString(),
                    TradingClass = version >= 32 ? ReadString() : string.Empty
                },
                Order =
                {
                    OrderId = orderId,
                    TradeAction = ReadStringEnum<TradeAction>(),
                    TotalQuantity = Read<double>(),
                    OrderType = ReadStringEnum<OrderType>(),
                    LimitPrice = Read<double?>(),
                    AuxPrice = Read<double?>(),
                    TimeInForce = ReadStringEnum<TimeInForce>(),
                    OcaGroup = ReadString(),
                    Account = ReadString(),
                    OpenClose = ReadStringEnum<OrderOpenClose>(),
                    Origin = Read<OrderOrigin>(),
                    OrderRef = ReadString(),
                    ClientId = Read<int>(),
                    PermanentId = Read<int>(),
                    OutsideRegularTradingHours = Read<bool>(),
                    Hidden = Read<bool>(),
                    DiscretionaryAmount = Read<double>(),
                    GoodAfterTime = ReadString()
                }
            };

            ReadString(); // skip deprecated sharesAllocation field

            var order = openOrder.Order;

            order.FinancialAdvisorGroup = ReadString();
            order.FinancialAdvisorMethod = ReadStringEnum<FinancialAdvisorAllocationMethod>();
            order.FinancialAdvisorPercentage = ReadString();
            order.FinancialAdvisorProfile = ReadString();

            if (config.SupportsServerVersion(ServerVersion.ModelsSupport))
                order.ModelCode = ReadString();

            order.GoodUntilDate = ReadString();

            order.Rule80A = ReadStringEnum<AgentDescription>();
            order.PercentOffset = Read<double?>();
            order.SettlingFirm = ReadString();
            order.ShortSaleSlot = Read<ShortSaleSlot>();
            order.DesignatedLocation = ReadString();
            order.ExemptCode = Read<int>();
            order.AuctionStrategy = Read<AuctionStrategy>();
            order.StartingPrice = Read<double?>();
            order.StockReferencePrice = Read<double?>();
            order.Delta = Read<double?>();
            order.StockRangeLower = Read<double?>();
            order.StockRangeUpper = Read<double?>();
            order.DisplaySize = Read<int>();

            order.BlockOrder = Read<bool>();
            order.SweepToFill = Read<bool>();
            order.AllOrNone = Read<bool>();
            order.MinimumQuantity = Read<int?>();
            order.OcaType = Read<OcaType>();
            order.ElectronicTradeOnly = Read<bool>();
            order.FirmQuoteOnly = Read<bool>();
            order.NbboPriceCap = Read<double?>();

            order.ParentId = Read<int>();
            order.TriggerMethod = Read<TriggerMethod>();

            order.Volatility = Read<double?>();
            order.VolatilityType = Read<VolatilityType>();

            order.DeltaNeutralOrderType = ReadString();
            order.DeltaNeutralAuxPrice = Read<double?>();

            if (!string.IsNullOrEmpty(order.DeltaNeutralOrderType))
            {
                if (version >= 27)
                {
                    order.DeltaNeutralContractId = Read<int>();
                    order.DeltaNeutralSettlingFirm = ReadString();
                    order.DeltaNeutralClearingAccount = ReadString();
                    order.DeltaNeutralClearingIntent = ReadString();
                }
                if (version >= 31)
                {
                    order.DeltaNeutralOpenClose = ReadString();
                    order.DeltaNeutralShortSale = Read<bool>();
                    order.DeltaNeutralShortSaleSlot = Read<int>();
                    order.DeltaNeutralDesignatedLocation = ReadString();
                }
            }

            order.ContinuousUpdate = Read<int>();
            order.ReferencePriceType = Read<ReferencePriceType>();

            order.TrailingStopPrice = Read<double?>();
            if (version >= 30)
                order.TrailingStopPercent = Read<double?>();

            order.BasisPoints = Read<double?>();
            order.BasisPointsType = Read<int?>();
            openOrder.Contract.ComboLegsDescription = ReadString();

            if (version >= 29)
            {
                var n = Read<int>();
                for (var i = 0; i < n; i++)
                {
                    openOrder.Contract.ComboLegs.Add(new ContractComboLeg
                    {
                        ContractId = Read<int>(),
                        Ratio = Read<int>(),
                        TradeAction = ReadStringEnum<TradeAction>(),
                        Exchange = ReadString(),
                        OpenClose = Read<ComboOpenClose>(),
                        ComboShortSaleSlot = Read<ComboShortSaleSlot>(),
                        DesignatedLocation = ReadString(),
                        ExemptCode = Read<int>()
                    });
                }

                n = Read<int>();
                for (var i = 0; i < n; i++)
                    order.ComboLegs.Add(new OrderComboLeg { Price = Read<double?>() });
            }

            if (version >= 26)
            {
                var n = Read<int>();
                for (var i = 0; i < n; i++)
                    order.SmartComboRoutingParams.Add(new Tag { Name = ReadString(), Value = ReadString() });
            }

            if (version >= 15)
            {
                if (version >= 20)
                {
                    order.ScaleInitLevelSize = Read<int?>();
                    order.ScaleSubsLevelSize = Read<int?>();
                }
                else
                {
                    ReadString();
                    order.ScaleInitLevelSize = Read<int?>();
                }
                order.ScalePriceIncrement = Read<double?>();
            }

            if (version >= 28 && order.ScalePriceIncrement > 0)
            {
                order.ScalePriceAdjustValue = Read<double?>();
                order.ScalePriceAdjustInterval = Read<int?>();
                order.ScaleProfitOffset = Read<double?>();
                order.ScaleAutoReset = Read<bool>();
                order.ScaleInitPosition = Read<int?>();
                order.ScaleInitFillQty = Read<int?>();
                order.ScaleRandomPercent = Read<bool>();
            }

            if (version >= 24)
            {
                order.HedgeType = ReadStringEnum<HedgeType>();
                if (order.HedgeType != HedgeType.Undefined)
                    order.HedgeParam = ReadString();
            }

            if (version >= 25)
                order.OptOutSmartRouting = Read<bool>();

            if (version >= 19)
            {
                order.ClearingAccount = ReadString();
                order.ClearingIntent = ReadStringEnum<ClearingIntent>();
            }

            if (version >= 19)
                order.NotHeld = Read<bool>();

            if (version >= 20 && Read<bool>())
            {
                openOrder.Contract.Undercomp = new UnderComp
                {
                    ContractId = Read<int>(),
                    Delta = Read<double>(),
                    Price = Read<double>()
                };
            }

            if (version >= 21)
            {
                order.AlgoStrategy = ReadString();
                if (!string.IsNullOrEmpty(order.AlgoStrategy))
                {
                    var n = Read<int>();
                    for (var i = 0; i < n; i++)
                        order.AlgoParams.Add(new Tag { Name = ReadString(), Value = ReadString() });
                }
            }

            if (version >= 33)
                order.Solicited = Read<bool>();

            order.WhatIf = Read<bool>();
            openOrder.Status = ReadStringEnum<OrderStatus>();
            openOrder.InitialMargin = ReadString();
            openOrder.MaintenanceMargin = ReadString();
            openOrder.EquityWithLoan = ReadString();
            openOrder.Commission = Read<double?>();
            openOrder.MinimumCommission = Read<double?>();
            openOrder.MaximumCommission = Read<double?>();
            openOrder.CommissionCurrency = ReadString();
            openOrder.WarningText = ReadString();

            if (version >= 34)
            {
                order.RandomizeSize = Read<bool>();
                order.RandomizePrice = Read<bool>();
            }

            if (config.SupportsServerVersion(ServerVersion.PeggedToBenchmark))
            {
                if (order.OrderType == OrderType.PeggedToBenchmark)
                {
                    order.ReferenceContractId = Read<int>();
                    order.IsPeggedChangeAmountDecrease = Read<bool>();
                    order.PeggedChangeAmount = Read<double?>();
                    order.ReferenceChangeAmount = Read<double?>();
                    order.ReferenceExchange = ReadString();
                }

                var n = Read<int>();
                if (n > 0)
                {
                    for (var i = 0; i < n; i++)
                    {
                        var orderConditionType = Read<OrderConditionType>();
                        var condition = OrderCondition.Create(orderConditionType);
                        condition.Deserialize(this);
                        order.Conditions.Add(condition);
                    }
                    order.ConditionsIgnoreRegularTradingHours = Read<bool>();
                    order.ConditionsCancelOrder = Read<bool>();
                }

                order.AdjustedOrderType = ReadString();
                order.TriggerPrice = Read<double?>();
                order.TrailingStopPrice = Read<double?>();
                order.LmtPriceOffset = Read<double?>();
                order.AdjustedStopPrice = Read<double?>();
                order.AdjustedStopLimitPrice = Read<double?>();
                order.AdjustedTrailingAmount = Read<double?>();
                order.AdjustableTrailingUnit = Read<int>();
            }

            if (config.SupportsServerVersion(ServerVersion.SoftDollarTier))
            {
                order.SoftDollarTier = new SoftDollarTier
                {
                    Name = ReadString(),
                    Value = ReadString(),
                    DisplayName = ReadString()
                };
            }

            if (config.SupportsServerVersion(ServerVersion.CashQty))
                order.CashQty = Read<double?>();

            return openOrder;
        }

        private Execution Execution()
        {
            var version = RequireVersion(10);
            var requestId = Read<int>();
            var orderId = Read<int>();
            var execution = new Execution
            {
                Contract =
                {
                    ContractId = Read<int>(),
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Multiplier = ReadString(),
                    Exchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString(),
                    TradingClass = version >= 10 ? ReadString() : string.Empty
                },
                RequestId = requestId,
                OrderId = orderId,
                ExecutionId = ReadString(),
                Time = ReadString(),
                Account = ReadString(),
                Exchange = ReadString(),
                Side = ReadStringEnum<ExecutionSide>(),
                Shares = Read<double>(),
                Price = Read<double>(),
                PermanentId = Read<int>(),
                ClientId = Read<int>(),
                Liquidation = Read<int>(),
                CumulativeQuantity = Read<double>(),
                AveragePrice = Read<double>(),
                OrderReference = ReadString(),
                EconomicValueRule = ReadString(),
                EconomicValueMultiplier = Read<double>()
            };

            executions[execution.ExecutionId] = execution;

            return execution;
        }

        // Store executions by executionId so that they can be associated with CommissionReport (above).
        // This assumes that CommissionReport always follows Execution.
        private readonly Dictionary<string, Execution> executions = new Dictionary<string, Execution>();

        private HistoricalBars HistoricalData() // a one-shot deal
        {
            RequireVersion(3);

            return new HistoricalBars
            {
                RequestId = Read<int>(),
                Start = Read<LocalDateTime>(),
                End = Read<LocalDateTime>(),
                Bars = GetBars(Read<int>())
            };

            List<HistoricalBar> GetBars(int n)
            {
                var list = new List<HistoricalBar>(n);
                for (var i = 0; i < n; i++)
                {
                    list.Add(new HistoricalBar
                    {
                        Date = Read<LocalDateTime>(),
                        Open = Read<double>(),
                        High = Read<double>(),
                        Low = Read<double>(),
                        Close = Read<double>(),
                        Volume = Read<int>(),
                        WeightedAveragePrice = Read<double>(),
                        Count = Read<int>()
                    });
                }
                return list;
            }
        }

        private ScannerData ScannerData() // a one-shot deal
        {
            RequireVersion(3);
            return new ScannerData
            {
                RequestId = Read<int>(),
                Items = GetItems(Read<int>())
            };

            List<ScannerDataItem> GetItems(int n)
            {
                var list = new List<ScannerDataItem>(n);
                for (var i = 0; i < n; i++)
                {
                    var item = new ScannerDataItem
                    {
                        Rank = Read<int>(),
                        ContractDetails =
                        {
                            Contract =
                            {
                                ContractId = Read<int>(),
                                Symbol = ReadString(),
                                SecurityType = ReadStringEnum<SecurityType>(),
                                LastTradeDateOrContractMonth = ReadString(),
                                Strike = Read<double>(),
                                Right = ReadStringEnum<RightType>(),
                                Exchange = ReadString(),
                                Currency = ReadString(),
                                LocalSymbol = ReadString(),
                            },
                            MarketName = ReadString()
                        }
                    };
                    item.ContractDetails.Contract.TradingClass = ReadString();
                    item.Distance = ReadString();
                    item.Benchmark = ReadString();
                    item.Projection = ReadString();
                    item.ComboLegs = ReadString();
                    list.Add(item);
                }
                return list;
            }
        }

        private ContractDetails ContractDetails()
        {
            RequireVersion(8);
            var cd = new ContractDetails
            {
                RequestId = Read<int>(),
                Contract =
                {
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Exchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString()
                },
                MarketName = ReadString()
            };
            cd.Contract.TradingClass = ReadString();
            cd.Contract.ContractId = Read<int>();
            cd.MinimumTick = Read<double>();
            cd.Contract.Multiplier = ReadString();
            cd.OrderTypes = ReadString();
            cd.ValidExchanges = ReadString();
            cd.PriceMagnifier = Read<int>();
            cd.UnderlyingContractId = Read<int>();
            cd.LongName = ReadString();
            cd.Contract.PrimaryExchange = ReadString();
            cd.ContractMonth = ReadString();
            cd.Industry = ReadString();
            cd.Category = ReadString();
            cd.Subcategory = ReadString();
            cd.TimeZoneId = ReadString();
            cd.TradingHours = ReadString();
            cd.LiquidHours = ReadString();
            cd.EconomicValueRule = ReadString();
            cd.EconomicValueMultiplier = Read<double>();
            cd.SecurityIds = GetTags(Read<int>());
            return cd;

            List<Tag> GetTags(int n) =>
                Enumerable.Repeat(new Tag { Name = ReadString(), Value = ReadString() }, n).ToList();
        }

        private ContractDetails BondContractDetails()
        {
            var version = RequireVersion(6);
            var cd = new ContractDetails
            {
                RequestId = Read<int>(),
                Contract =
                {
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>()
                },
                Cusip = ReadString(),
                Coupon = Read<double>(),
                Maturity = ReadString(),
                IssueDate = ReadString(),
                CreditRatings = ReadString(),
                BondType = ReadString(),
                CouponType = ReadString(),
                Convertible = Read<bool>(),
                Callable = Read<bool>(),
                Putable = Read<bool>(),
                DescriptionAppend = ReadString()
            };
            cd.Contract.Exchange = ReadString();
            cd.Contract.Currency = ReadString();
            cd.MarketName = ReadString();
            cd.Contract.TradingClass = ReadString();
            cd.Contract.ContractId = Read<int>();
            cd.MinimumTick = Read<double>();
            cd.OrderTypes = ReadString();
            cd.ValidExchanges = ReadString();
            cd.NextOptionDate = ReadString();
            cd.NextOptionType = ReadString();
            cd.NextOptionPartial = Read<bool>();
            cd.Notes = ReadString();
            cd.LongName = ReadString();
            if (version >= 6)
            {
                cd.EconomicValueRule = ReadString();
                cd.EconomicValueMultiplier = Read<double>();
            }
            if (version >= 5)
                cd.SecurityIds = GetTags(Read<int>());
            return cd;

            List<Tag> GetTags(int n) =>
                Enumerable.Repeat(new Tag { Name = ReadString(), Value = ReadString() }, n).ToList();
        }

        private PositionMulti PositionMulti()
        {
            IgnoreVersion();
            return new PositionMulti
            {
                RequestId = Read<int>(),
                Account = ReadString(),
                Contract =
                {
                    ContractId = Read<int>(),
                    Symbol = ReadString(),
                    SecurityType = ReadStringEnum<SecurityType>(),
                    LastTradeDateOrContractMonth = ReadString(),
                    Strike = Read<double>(),
                    Right = ReadStringEnum<RightType>(),
                    Multiplier = ReadString(),
                    Exchange = ReadString(),
                    Currency = ReadString(),
                    LocalSymbol = ReadString(),
                    TradingClass = ReadString()
                },
                Pos = Read<double>(),
                AvgCost = Read<double>(),
                ModelCode = ReadString()
            };
        }

        private PositionMultiEnd PositionMultiEnd()
        {
            IgnoreVersion();
            return new PositionMultiEnd { RequestId = Read<int>() };
        }

        private AccountUpdateMulti AccountUpdateMulti()
        {
            IgnoreVersion();
            return new AccountUpdateMulti
            {
                RequestId = Read<int>(),
                Account = ReadString(),
                ModelCode = ReadString(),
                Key = ReadString(),
                Value = ReadString(),
                Currency = ReadString()
            };
        }

        private AccountUpdateMultiEnd AccountUpdateMultiEnd()
        {
            IgnoreVersion();
            return new AccountUpdateMultiEnd { RequestId = Read<int>() };
        }

        private SecurityDefinitionOptionParameter SecurityDefinitionOptionParameter()
        {
            // no more versions!
            return new SecurityDefinitionOptionParameter
            {
                RequestId = Read<int>(),
                Exchange = ReadString(),
                UnderlyingContractId = Read<int>(),
                TradingClass = ReadString(),
                Multiplier = ReadString(),
                Expirations = GetStrings(Read<int>()),
                Strikes = GetStrings(Read<int>())
            };

            List<string> GetStrings(int n) => Enumerable.Repeat(ReadString(), n).ToList();
        }

        private SecurityDefinitionOptionParameterEnd SecurityDefinitionOptionParameterEnd() =>
            new SecurityDefinitionOptionParameterEnd { RequestId = Read<int>() };

        private SoftDollarTiers SoftDollarTiers()
        {
            return new SoftDollarTiers
            {
                RequestId = Read<int>(),
                Tiers = GetList(Read<int>())
            };

            List<SoftDollarTier> GetList(int n) =>
                Enumerable.Repeat(new SoftDollarTier
                {
                    Name = ReadString(),
                    Value = ReadString(),
                    DisplayName = ReadString()
                }, n).ToList();
        }

        private IReadOnlyList<FamilyCode> FamilyCodes()
        {
            var n = Read<int>();
            return Enumerable.Repeat(new FamilyCode { AccountId = ReadString(), FamilyCodeStr = ReadString()}, n).ToList();
        }

        private ContractDescriptions SymbolSamples()
        {
            return new ContractDescriptions
            {
                RequestId = Read<int>(),
                Descriptions = GetContractDescriptions(Read<int>())
            };

            List<ContractDescription> GetContractDescriptions(int n)
            {
                var list = new List<ContractDescription>(n);
                for (var i = 0; i < n; i++)
                {
                    list.Add(new ContractDescription
                    {
                        Contract =
                        {
                            ContractId = Read<int>(),
                            Symbol = ReadString(),
                            SecurityType = Read<SecurityType>(),
                            PrimaryExchange = ReadString(),
                            Currency = ReadString()
                        },
                        DerivativeSecTypes = GetStrings(Read<int>())
                    });
                }
                return list;
            }

            List<string> GetStrings(int n) => Enumerable.Repeat(ReadString(), n).ToList();
        }

        private List<DepthMktDataDescription> MarketDepthExchanges()
        {
            var n = Read<int>();
            return Enumerable.Repeat(GetDepth(), n).ToList();

            DepthMktDataDescription GetDepth()
            {
                var d = new DepthMktDataDescription { Exchange = ReadString(), SecType = ReadString() };
                if (config.SupportsServerVersion(ServerVersion.ServiceDataType))
                {
                    d.ListingExch = ReadString();
                    d.ServiceDataTyp = ReadString();
                    d.AggGroup = Read<int?>();
                }
                else
                {
                    d.ListingExch = "";
                    d.ServiceDataTyp = Read<bool>() ? "Deep2" : "Deep";
                }
                return d;
            }
        }

        /// <summary>
        /// A response to a market data request.
        /// </summary>
        private TickReqParams TickReqParams() =>
            new TickReqParams
            {
                RequestId = Read<int>(),
                MinTick = Read<double>(),
                BboExchange = ReadString(),
                SnapshotPermissions = Read<int>()
            };

        /// <summary>
        /// A response to RequestSmartComponents.
        /// The tick types 'bidExch', 'askExch', 'lastExch' are used to identify the source of a quote.
        /// This result provides a map relating single letters to exchange names.
        /// </summary>
        private SmartComponents SmartComponents()
        {
            return new SmartComponents
            {
                RequestId = Read<int>(),
                Map = GetMap(Read<int>())
            };

            Dictionary<int, KeyValuePair<string, char>> GetMap(int n)
            {
                var map = new Dictionary<int, KeyValuePair<string, char>>(n);
                for (var i = 0; i < n; i++)
                {
                    var bitNumber = Read<int>();
                    var exchange = ReadString();
                    var exchangeLetter = Read<char>();
                    map.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
                }
                return map;
            }
        }

        private NewsArticle NewsArticle() =>
            new NewsArticle
            {
                RequestId = Read<int>(),
                ArticleType = Read<int>(),
                ArticleText = ReadString()
            };

        private TickNews TickNews() =>
            new TickNews
            {
                RequestId = Read<int>(),
                TimeStamp = Read<long>(),
                ProviderCode = ReadString(),
                ArticleId = ReadString(),
                Headline = ReadString(),
                ExtraData = ReadString()
            };

        private IReadOnlyList<NewsProvider> NewsProviders()
        {
            var n = Read<int>();
            return Enumerable.Repeat(new NewsProvider { Code = ReadString(), Name = ReadString()}, n).ToList();
        }

        private HistoricalNews HistoricalNews() =>
            new HistoricalNews
            {
                RequestId = Read<int>(),
                Time = ReadString(),
                ProviderCode = ReadString(),
                ArticleId = ReadString(),
                Headline = ReadString()
            };

        private HistoricalNewsEnd HistoricalNewsEnd() =>
            new HistoricalNewsEnd
            {
                RequestId = Read<int>(),
                HasMore = Read<bool>()
            };

        private HeadTimestamp HeadTimestamp() =>
            new HeadTimestamp
            {
                RequestId = Read<int>(),
                HeadTimeStamp = ReadString()
            };

        private HistogramData HistogramData()
        {
            return new HistogramData
            {
                RequestId = Read<int>(),
                Data = GetData(Read<int>())
            };

            List<(double,long)> GetData(int n) => Enumerable.Repeat((Read<double>(), Read<long>()), n).ToList();
        }
    }
}
