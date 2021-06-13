namespace InterReact
{
    public sealed class OpenOrder : IOrder, IHasOrderId
    {
        public Order Order { get; }
        public int OrderId { get; }

        public Contract Contract { get; }

        public OrderStatus Status { get; }

        /// <summary>
        /// Initial margin requirement for the order.
        /// </summary>
        public string InitialMarginBefore { get; } = "";

        /// <summary>
        /// Maintenance margin requirement for the order.
        /// Shows the impact the order would have on your initial margin.
        /// </summary>
        public string MaintenanceMarginBefore { get; } = "";

        /// <summary>
        /// Shows the impact the order would have on your equity with loan value.
        /// </summary>
        public string EquityWithLoanBefore { get; }
        public string InitMarginChange { get; } = "";
        public string MaintMarginChange { get; } = "";
        public string EquityWithLoanChange { get; } = "";
        public string InitMarginAfter { get; }
        public string MaintMarginAfter { get; }
        public string EquityWithLoanAfter { get; } = "";

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
            int messageVersion = c.RequireVersion(17);
            int orderId = c.ReadInt();

            Contract = new Contract
            {
                ContractId = c.ReadInt(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.ReadDouble(),
                Right = c.ReadStringEnum<OptionRightType>(),
                Multiplier = messageVersion >= 32 ? c.ReadString() : "",
                Exchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = messageVersion >= 32 ? c.ReadString() : ""
            };
            Order = new Order
            {
                OrderId = orderId,
                TradeAction = c.ReadStringEnum<TradeAction>(),
                TotalQuantity = c.Config.SupportsServerVersion(ServerVersion.FractionalPositions) ? c.ReadDouble() : c.ReadInt(),
                OrderType = c.ReadStringEnum<OrderType>(),
                LimitPrice = messageVersion < 29 ? c.ReadDouble() : c.ReadDoubleNullable(),
                AuxPrice = messageVersion < 30 ? c.ReadDouble() : c.ReadDoubleNullable(),
                TimeInForce = c.ReadStringEnum<TimeInForce>(),
                OcaGroup = c.ReadString(),
                Account = c.ReadString(),
                OpenClose = c.ReadStringEnum<OrderOpenClose>(),
                Origin = c.ReadEnum<OrderOrigin>(),
                OrderRef = c.ReadString(),
                ClientId = c.ReadInt(),
                PermanentId = c.ReadInt(),
                OutsideRegularTradingHours = c.ReadBool(),
                Hidden = c.ReadBool(),
                DiscretionaryAmount = c.ReadDouble(),
                GoodAfterTime = c.ReadString()
            };

            OrderId = Order.OrderId;

            c.ReadString(); // skip deprecated sharesAllocation field

            Order.FinancialAdvisorGroup = c.ReadString();
            Order.FinancialAdvisorMethod = c.ReadStringEnum<FinancialAdvisorAllocationMethod>();
            Order.FinancialAdvisorPercentage = c.ReadString();
            Order.FinancialAdvisorProfile = c.ReadString();

            if (c.Config.SupportsServerVersion(ServerVersion.ModelsSupport))
                Order.ModelCode = c.ReadString();

            Order.GoodUntilDate = c.ReadString();
            Order.Rule80A = c.ReadStringEnum<AgentDescription>();
            Order.PercentOffset = c.ReadDoubleNullable();
            Order.SettlingFirm = c.ReadString();
            Order.ShortSaleSlot = c.ReadEnum<ShortSaleSlot>();
            Order.DesignatedLocation = c.ReadString();
            Order.ExemptCode = c.ReadInt();
            Order.AuctionStrategy = c.ReadEnum<AuctionStrategy>();
            Order.StartingPrice = c.ReadDoubleNullable();
            Order.StockReferencePrice = c.ReadDoubleNullable();
            Order.Delta = c.ReadDoubleNullable();
            Order.StockRangeLower = c.ReadDoubleNullable();
            Order.StockRangeUpper = c.ReadDoubleNullable();
            Order.DisplaySize = c.ReadInt();

            Order.BlockOrder = c.ReadBool();
            Order.SweepToFill = c.ReadBool();
            Order.AllOrNone = c.ReadBool();
            Order.MinimumQuantity = c.ReadIntNullable();
            Order.OcaType = c.ReadEnum<OcaType>();
            Order.ElectronicTradeOnly = c.ReadBool();
            Order.FirmQuoteOnly = c.ReadBool();
            Order.NbboPriceCap = c.ReadDoubleNullable();

            Order.ParentId = c.ReadInt();
            Order.TriggerMethod = c.ReadEnum<TriggerMethod>();

            Order.Volatility = c.ReadDoubleNullable();
            Order.VolatilityType = c.ReadEnum<VolatilityType>();

            Order.DeltaNeutralOrderType = c.ReadString();
            Order.DeltaNeutralAuxPrice = c.ReadDoubleNullable();

            if (!string.IsNullOrEmpty(Order.DeltaNeutralOrderType))
            {
                if (messageVersion >= 27)
                {
                    Order.DeltaNeutralContractId = c.ReadInt();
                    Order.DeltaNeutralSettlingFirm = c.ReadString();
                    Order.DeltaNeutralClearingAccount = c.ReadString();
                    Order.DeltaNeutralClearingIntent = c.ReadString();
                }
                if (messageVersion >= 31)
                {
                    Order.DeltaNeutralOpenClose = c.ReadString();
                    Order.DeltaNeutralShortSale = c.ReadBool();
                    Order.DeltaNeutralShortSaleSlot = c.ReadInt();
                    Order.DeltaNeutralDesignatedLocation = c.ReadString();
                }
            }

            Order.ContinuousUpdate = c.ReadInt();
            Order.ReferencePriceType = c.ReadEnum<ReferencePriceType>();

            Order.TrailingStopPrice = c.ReadDoubleNullable();
            if (messageVersion >= 30)
                Order.TrailingStopPercent = c.ReadDoubleNullable();

            Order.BasisPoints = c.ReadDoubleNullable();
            Order.BasisPointsType = c.ReadIntNullable();
            Contract.ComboLegsDescription = c.ReadString();

            if (messageVersion >= 29)
            {
                int n = c.ReadInt();
                for (var i = 0; i < n; i++)
                    Contract.ComboLegs.Add(new ContractComboLeg(c));

                n = c.ReadInt();
                for (var i = 0; i < n; i++)
                    Order.ComboLegs.Add(new OrderComboLeg(c.ReadDoubleNullable()));
            }

            if (messageVersion >= 26)
                c.AddTagsToList(Order.SmartComboRoutingParams);

            if (messageVersion >= 15)
            {
                if (messageVersion >= 20)
                {
                    Order.ScaleInitLevelSize = c.ReadIntNullable();
                    Order.ScaleSubsLevelSize = c.ReadIntNullable();
                }
                else
                {
                    c.ReadString();
                    Order.ScaleInitLevelSize = c.ReadIntNullable();
                }
                Order.ScalePriceIncrement = c.ReadDoubleNullable();
            }

            if (messageVersion >= 28 && Order.ScalePriceIncrement > 0)
            {
                Order.ScalePriceAdjustValue = c.ReadDoubleNullable();
                Order.ScalePriceAdjustInterval = c.ReadIntNullable();
                Order.ScaleProfitOffset = c.ReadDoubleNullable();
                Order.ScaleAutoReset = c.ReadBool();
                Order.ScaleInitPosition = c.ReadIntNullable();
                Order.ScaleInitFillQty = c.ReadIntNullable();
                Order.ScaleRandomPercent = c.ReadBool();
            }

            if (messageVersion >= 24)
            {
                Order.HedgeType = c.ReadStringEnum<HedgeType>();
                if (Order.HedgeType != HedgeType.Undefined)
                    Order.HedgeParam = c.ReadString();
            }

            if (messageVersion >= 25)
                Order.OptOutSmartRouting = c.ReadBool();

            if (messageVersion >= 19)
            {
                Order.ClearingAccount = c.ReadString();
                Order.ClearingIntent = c.ReadStringEnum<ClearingIntent>();
            }

            if (messageVersion >= 22)
                Order.NotHeld = c.ReadBool();

            if (messageVersion >= 20 && c.ReadBool())
                Contract.DeltaNeutralContract = new DeltaNeutralContract(c, false);

            if (messageVersion >= 21)
            {
                Order.AlgoStrategy = c.ReadString();
                if (!string.IsNullOrEmpty(Order.AlgoStrategy))
                    c.AddTagsToList(Order.AlgoParams);
            }

            if (messageVersion >= 33)
                Order.Solicited = c.ReadBool();

            Order.WhatIf = c.ReadBool();
            Status = c.ReadStringEnum<OrderStatus>();

            InitMarginAfter = c.ReadString();
            MaintMarginAfter = c.ReadString();
            EquityWithLoanBefore = c.ReadString();
            Commission = c.ReadDoubleNullable();
            MinimumCommission = c.ReadDoubleNullable();
            MaximumCommission = c.ReadDoubleNullable();
            CommissionCurrency = c.ReadString();
            WarningText = c.ReadString();

            if (messageVersion >= 34)
            {
                Order.RandomizeSize = c.ReadBool();
                Order.RandomizePrice = c.ReadBool();
            }

            if (c.Config.SupportsServerVersion(ServerVersion.PeggedToBenchmark))
            {
                if (Order.OrderType == OrderType.PeggedToBenchmark)
                {
                    Order.ReferenceContractId = c.ReadInt();
                    Order.IsPeggedChangeAmountDecrease = c.ReadBool();
                    Order.PeggedChangeAmount = c.ReadDoubleNullable();
                    Order.ReferenceChangeAmount = c.ReadDoubleNullable();
                    Order.ReferenceExchange = c.ReadString();
                }

                int n = c.ReadInt();
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        OrderConditionType orderConditionType = c.ReadEnum<OrderConditionType>();
                        OrderCondition condition = OrderCondition.Create(orderConditionType);
                        condition.Deserialize(c);
                        Order.Conditions.Add(condition);
                    }
                    Order.ConditionsIgnoreRegularTradingHours = c.ReadBool();
                    Order.ConditionsCancelOrder = c.ReadBool();
                }

                Order.AdjustedOrderType = c.ReadString();
                Order.TriggerPrice = c.ReadDoubleNullable();
                Order.TrailingStopPrice = c.ReadDoubleNullable();
                Order.LmtPriceOffset = c.ReadDoubleNullable();
                Order.AdjustedStopPrice = c.ReadDoubleNullable();
                Order.AdjustedStopLimitPrice = c.ReadDoubleNullable();
                Order.AdjustedTrailingAmount = c.ReadDoubleNullable();
                Order.AdjustableTrailingUnit = c.ReadInt();
            }

            if (c.Config.SupportsServerVersion(ServerVersion.SoftDollarTier))
                Order.SoftDollarTier = new SoftDollarTier(c);

            if (c.Config.SupportsServerVersion(ServerVersion.CashQty))
                Order.CashQty = c.ReadDoubleNullable();
        }
    }

    public sealed class OpenOrderEnd
    {
        internal OpenOrderEnd(ResponseReader c) => c.IgnoreVersion();
    }

}