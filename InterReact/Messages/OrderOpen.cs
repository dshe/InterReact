using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages.Conditions;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class OpenOrder : IHasOrderId
    {
        public Order Order { get; }
        public int OrderId => Order.OrderId;

        public Contract Contract { get; }

        public OrderStatus Status { get; }

        /// <summary>
        /// Initial margin requirement for the order.
        /// </summary>
        public string InitialMargin { get; }

        /// <summary>
        /// Maintenance margin requirement for the order.
        /// Shows the impact the order would have on your initial margin.
        /// </summary>
        public string MaintenanceMargin { get; }

        /// <summary>
        /// Shows the impact the order would have on your equity with loan value.
        /// </summary>
        public string EquityWithLoan { get; }

        public double? Commission { get; }
        /// <summary>
        /// Used in conjunction with the maxCommission field, this defines the lowest end of the possible range into which the actual order commission will fall.
        /// </summary>
        public double? MinimumCommission { get; }
        /// <summary>
        /// Used in conjunction with the minCommission field, this defines the highest end of the possible range into which the actual order commission will fall.
        /// </summary>
        public double? MaximumCommission { get; }

        public string CommissionCurrency { get; }

        public string WarningText { get; }

        internal OpenOrder(ResponseReader c) // the monster
        {
            var version = c.RequireVersion(17);
            var orderId = c.Read<int>();

            Contract = new Contract
            {
                ContractId = c.Read<int>(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.Read<double>(),
                Right = c.ReadStringEnum<RightType>(),
                Multiplier = version >= 32 ? c.ReadString() : string.Empty,
                Exchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = version >= 32 ? c.ReadString() : string.Empty
            };
            Order = new Order
            {
                OrderId = orderId,
                TradeAction = c.ReadStringEnum<TradeAction>(),
                TotalQuantity = c.Read<double>(),
                OrderType = c.ReadStringEnum<OrderType>(),
                LimitPrice = c.Read<double?>(),
                AuxPrice = c.Read<double?>(),
                TimeInForce = c.ReadStringEnum<TimeInForce>(),
                OcaGroup = c.ReadString(),
                Account = c.ReadString(),
                OpenClose = c.ReadStringEnum<OrderOpenClose>(),
                Origin = c.Read<OrderOrigin>(),
                OrderRef = c.ReadString(),
                ClientId = c.Read<int>(),
                PermanentId = c.Read<int>(),
                OutsideRegularTradingHours = c.Read<bool>(),
                Hidden = c.Read<bool>(),
                DiscretionaryAmount = c.Read<double>(),
                GoodAfterTime = c.ReadString()
            };

            c.ReadString(); // skip deprecated sharesAllocation field

            Order.FinancialAdvisorGroup = c.ReadString();
            Order.FinancialAdvisorMethod = c.ReadStringEnum<FinancialAdvisorAllocationMethod>();
            Order.FinancialAdvisorPercentage = c.ReadString();
            Order.FinancialAdvisorProfile = c.ReadString();

            if (c.Config.SupportsServerVersion(ServerVersion.ModelsSupport))
                Order.ModelCode = c.ReadString();

            Order.GoodUntilDate = c.ReadString();


            Order.Rule80A = c.ReadStringEnum<AgentDescription>();
            Order.PercentOffset = c.Read<double?>();
            Order.SettlingFirm = c.ReadString();
            Order.ShortSaleSlot = c.Read<ShortSaleSlot>();
            Order.DesignatedLocation = c.ReadString();
            Order.ExemptCode = c.Read<int>();
            Order.AuctionStrategy = c.Read<AuctionStrategy>();
            Order.StartingPrice = c.Read<double?>();
            Order.StockReferencePrice = c.Read<double?>();
            Order.Delta = c.Read<double?>();
            Order.StockRangeLower = c.Read<double?>();
            Order.StockRangeUpper = c.Read<double?>();
            Order.DisplaySize = c.Read<int>();

            Order.BlockOrder = c.Read<bool>();
            Order.SweepToFill = c.Read<bool>();
            Order.AllOrNone = c.Read<bool>();
            Order.MinimumQuantity = c.Read<int?>();
            Order.OcaType = c.Read<OcaType>();
            Order.ElectronicTradeOnly = c.Read<bool>();
            Order.FirmQuoteOnly = c.Read<bool>();
            Order.NbboPriceCap = c.Read<double?>();

            Order.ParentId = c.Read<int>();
            Order.TriggerMethod = c.Read<TriggerMethod>();

            Order.Volatility = c.Read<double?>();
            Order.VolatilityType = c.Read<VolatilityType>();

            Order.DeltaNeutralOrderType = c.ReadString();
            Order.DeltaNeutralAuxPrice = c.Read<double?>();

            if (!string.IsNullOrEmpty(Order.DeltaNeutralOrderType))
            {
                if (version >= 27)
                {
                    Order.DeltaNeutralContractId = c.Read<int>();
                    Order.DeltaNeutralSettlingFirm = c.ReadString();
                    Order.DeltaNeutralClearingAccount = c.ReadString();
                    Order.DeltaNeutralClearingIntent = c.ReadString();
                }
                if (version >= 31)
                {
                    Order.DeltaNeutralOpenClose = c.ReadString();
                    Order.DeltaNeutralShortSale = c.Read<bool>();
                    Order.DeltaNeutralShortSaleSlot = c.Read<int>();
                    Order.DeltaNeutralDesignatedLocation = c.ReadString();
                }
            }

            Order.ContinuousUpdate = c.Read<int>();
            Order.ReferencePriceType = c.Read<ReferencePriceType>();

            Order.TrailingStopPrice = c.Read<double?>();
            if (version >= 30)
                Order.TrailingStopPercent = c.Read<double?>();

            Order.BasisPoints = c.Read<double?>();
            Order.BasisPointsType = c.Read<int?>();
            Contract.ComboLegsDescription = c.ReadString();

            if (version >= 29)
            {
                var n = c.Read<int>();
                for (var i = 0; i < n; i++)
                    Contract.ComboLegs.Add(new ContractComboLeg(c));

                n = c.Read<int>();
                for (var i = 0; i < n; i++)
                    Order.ComboLegs.Add(new OrderComboLeg(c.Read<double?>()));
            }

            if (version >= 26)
            {
                var n = c.Read<int>();
                for (var i = 0; i < n; i++)
                    Order.SmartComboRoutingParams.Add(new Tag(c));
            }

            if (version >= 15)
            {
                if (version >= 20)
                {
                    Order.ScaleInitLevelSize = c.Read<int?>();
                    Order.ScaleSubsLevelSize = c.Read<int?>();
                }
                else
                {
                    c.ReadString();
                    Order.ScaleInitLevelSize = c.Read<int?>();
                }
                Order.ScalePriceIncrement = c.Read<double?>();
            }

            if (version >= 28 && Order.ScalePriceIncrement > 0)
            {
                Order.ScalePriceAdjustValue = c.Read<double?>();
                Order.ScalePriceAdjustInterval = c.Read<int?>();
                Order.ScaleProfitOffset = c.Read<double?>();
                Order.ScaleAutoReset = c.Read<bool>();
                Order.ScaleInitPosition = c.Read<int?>();
                Order.ScaleInitFillQty = c.Read<int?>();
                Order.ScaleRandomPercent = c.Read<bool>();
            }

            if (version >= 24)
            {
                Order.HedgeType = c.ReadStringEnum<HedgeType>();
                if (Order.HedgeType != HedgeType.Undefined)
                    Order.HedgeParam = c.ReadString();
            }

            if (version >= 25)
                Order.OptOutSmartRouting = c.Read<bool>();

            if (version >= 19)
            {
                Order.ClearingAccount = c.ReadString();
                Order.ClearingIntent = c.ReadStringEnum<ClearingIntent>();
            }

            if (version >= 19)
                Order.NotHeld = c.Read<bool>();

            if (version >= 20 && c.Read<bool>())
                Contract.Undercomp = new UnderComp(c, true);

            if (version >= 21)
            {
                Order.AlgoStrategy = c.ReadString();
                if (!string.IsNullOrEmpty(Order.AlgoStrategy))
                {
                    var n = c.Read<int>();
                    for (var i = 0; i < n; i++)
                        Order.AlgoParams.Add(new Tag(c));
                }
            }

            if (version >= 33)
                Order.Solicited = c.Read<bool>();

            Order.WhatIf = c.Read<bool>();
            Status = c.ReadStringEnum<OrderStatus>();
            InitialMargin = c.ReadString();
            MaintenanceMargin = c.ReadString();
            EquityWithLoan = c.ReadString();
            Commission = c.Read<double?>();
            MinimumCommission = c.Read<double?>();
            MaximumCommission = c.Read<double?>();
            CommissionCurrency = c.ReadString();
            WarningText = c.ReadString();

            if (version >= 34)
            {
                Order.RandomizeSize = c.Read<bool>();
                Order.RandomizePrice = c.Read<bool>();
            }

            if (c.Config.SupportsServerVersion(ServerVersion.PeggedToBenchmark))
            {
                if (Order.OrderType == OrderType.PeggedToBenchmark)
                {
                    Order.ReferenceContractId = c.Read<int>();
                    Order.IsPeggedChangeAmountDecrease = c.Read<bool>();
                    Order.PeggedChangeAmount = c.Read<double?>();
                    Order.ReferenceChangeAmount = c.Read<double?>();
                    Order.ReferenceExchange = c.ReadString();
                }

                var n = c.Read<int>();
                if (n > 0)
                {
                    for (var i = 0; i < n; i++)
                    {
                        var orderConditionType = c.Read<OrderConditionType>();
                        var condition = OrderCondition.Create(orderConditionType);
                        condition.Deserialize(c);
                        Order.Conditions.Add(condition);
                    }
                    Order.ConditionsIgnoreRegularTradingHours = c.Read<bool>();
                    Order.ConditionsCancelOrder = c.Read<bool>();
                }

                Order.AdjustedOrderType = c.ReadString();
                Order.TriggerPrice = c.Read<double?>();
                Order.TrailingStopPrice = c.Read<double?>();
                Order.LmtPriceOffset = c.Read<double?>();
                Order.AdjustedStopPrice = c.Read<double?>();
                Order.AdjustedStopLimitPrice = c.Read<double?>();
                Order.AdjustedTrailingAmount = c.Read<double?>();
                Order.AdjustableTrailingUnit = c.Read<int>();
            }

            if (c.Config.SupportsServerVersion(ServerVersion.SoftDollarTier))
                Order.SoftDollarTier = new SoftDollarTier(c);

            if (c.Config.SupportsServerVersion(ServerVersion.CashQty))
                Order.CashQty = c.Read<double?>();
        }
    }

    public sealed class OpenOrderEnd
    {
        internal OpenOrderEnd(ResponseReader c) => c.IgnoreVersion();
    }

}