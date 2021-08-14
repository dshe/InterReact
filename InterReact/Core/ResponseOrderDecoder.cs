namespace InterReact
{
    internal class OrderDecoder
    {
        private readonly ResponseReader R;
        private readonly Contract Contract;
        private readonly Order Order;
        private readonly OrderState OrderState;
        private readonly int MessageVersion;

        internal OrderDecoder(ResponseReader reader, Contract contract, Order order, OrderState orderState, int msgVersion)
        {
            R = reader;
            Contract = contract;
            Order = order;
            OrderState = orderState;
            MessageVersion = msgVersion;
        }

        internal void readOrderId() => Order.OrderId = R.ReadInt();
        internal void readAction() => Order.OrderAction = R.ReadStringEnum<OrderAction>();

        public void readContract()
        {
            if (MessageVersion >= 17)
                Contract.ContractId = R.ReadInt();

            Contract.Symbol = R.ReadString();
            Contract.SecurityType = R.ReadStringEnum<SecurityType>();
            Contract.LastTradeDateOrContractMonth = R.ReadString();
            Contract.Strike = R.ReadDouble();
            Contract.Right = R.ReadStringEnum<OptionRightType>();

            if (MessageVersion >= 32)
                Contract.Multiplier = R.ReadString();

            Contract.Exchange = R.ReadString();
            Contract.Currency = R.ReadString();

            if (MessageVersion >= 2)
                Contract.LocalSymbol = R.ReadString();

            if (MessageVersion >= 32)
                Contract.TradingClass = R.ReadString();
        }

        internal void readTotalQuantity() => Order.TotalQuantity = R.ReadDouble();
        internal void readOrderType() => Order.OrderType = R.ReadStringEnum<OrderType>();
        internal void readLmtPrice() => Order.LimitPrice = (MessageVersion < 29) ? R.ReadDouble() : R.ReadDoubleNullable();
        internal void readAuxPrice() => Order.AuxPrice = (MessageVersion < 30) ? R.ReadDouble() : R.ReadDoubleNullable();
        internal void readTIF() => Order.TimeInForce = R.ReadStringEnum<TimeInForce>();
        internal void readOcaGroup() => Order.OcaGroup = R.ReadString();
        internal void readAccount() => Order.Account = R.ReadString();
        internal void readOpenClose() => Order.OpenClose = R.ReadStringEnum<OrderOpenClose>();
        internal void readOrigin() => Order.Origin = R.ReadEnum<OrderOrigin>();
        internal void readOrderRef() => Order.OrderRef = R.ReadString();

        internal void readClientId()
        {
            if (MessageVersion >= 3)
                Order.ClientId = R.ReadInt();
        }

        internal void readPermId()
        {
            if (MessageVersion >= 4)
                Order.PermanentId = R.ReadInt();
        }

        internal void readOutsideRth()
        {
            if (MessageVersion >= 4)
            {
                if (MessageVersion < 18)
                    R.ReadString(); // will never happen, order.ignoreRth
                else
                    Order.OutsideRegularTradingHours = R.ReadBool();
            }
        }

        internal void readHidden()
        {
            if (MessageVersion >= 4)
                Order.Hidden = R.ReadInt() == 1;
        }

        internal void readDiscretionaryAmount()
        {
            if (MessageVersion >= 4)
                Order.DiscretionaryAmount = R.ReadDouble();
        }

        internal void readGoodAfterTime()
        {
            if (MessageVersion >= 5)
                Order.GoodAfterTime = R.ReadString();
        }

        internal void skipSharesAllocation()
        {
            if (MessageVersion >= 6)
                R.ReadString(); // skip deprecated sharesAllocation field
        }

        internal void readFAParams()
        {
            if (MessageVersion >= 7)
            {
                Order.FinancialAdvisorGroup = R.ReadString();
                Order.FinancialAdvisorMethod = R.ReadStringEnum<FinancialAdvisorAllocationMethod>();
                Order.FinancialAdvisorPercentage = R.ReadString();
                Order.FinancialAdvisorProfile = R.ReadString();
            }
        }

        internal void readModelCode()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.MODELS_SUPPORT))
                Order.ModelCode = R.ReadString();
        }

        internal void readGoodTillDate()
        {
            if (MessageVersion >= 8)
                Order.GoodUntilDate = R.ReadString();
        }

        internal void readRule80A()
        {
            if (MessageVersion >= 9)
                Order.Rule80A = R.ReadStringEnum<AgentDescription>();
        }

        internal void readPercentOffset()
        {
            if (MessageVersion >= 9)
                Order.PercentOffset = R.ReadDoubleNullable();
        }

        internal void readSettlingFirm()
        {
            if (MessageVersion >= 9)
                Order.SettlingFirm = R.ReadString();
        }

        internal void readShortSaleParams()
        {
            if (MessageVersion >= 9)
            {
                Order.ShortSaleSlot = R.ReadEnum<ShortSaleSlot>();
                Order.DesignatedLocation = R.ReadString();
                if ((int)R.Config.ServerVersionCurrent == 51)
                    R.ReadInt(); // exemptCode
                else if (MessageVersion >= 23)
                    Order.ExemptCode = R.ReadInt();
            }
        }

        internal void readAuctionStrategy()
        {
            if (MessageVersion >= 9)
                Order.AuctionStrategy = R.ReadEnum<AuctionStrategy>();
        }

        internal void readBoxOrderParams()
        {
            if (MessageVersion >= 9)
            {
                Order.StartingPrice = R.ReadDoubleNullable();
                Order.StockReferencePrice = R.ReadDoubleNullable();
                Order.Delta = R.ReadDoubleNullable();
            }
        }

        internal void readPegToStkOrVolOrderParams()
        {
            if (MessageVersion >= 9)
            {
                Order.StockRangeLower = R.ReadDoubleNullable();
                Order.StockRangeUpper = R.ReadDoubleNullable();
            }
        }

        internal void readDisplaySize()
        {
            if (MessageVersion >= 9)
                Order.DisplaySize = R.ReadIntNullable();
        }

        internal void readOldStyleOutsideRth()
        {
            if (MessageVersion >= 9 && MessageVersion < 18)
                R.ReadBool(); // will never happen order.rthOnly
        }

        internal void readBlockOrder()
        {
            if (MessageVersion >= 9)
                Order.BlockOrder = R.ReadBool();
        }

        internal void readSweepToFill()
        {
            if (MessageVersion >= 9)
                Order.SweepToFill = R.ReadBool();
        }

        internal void readAllOrNone()
        {
            if (MessageVersion >= 9)
                Order.AllOrNone = R.ReadBool();
        }

        internal void readMinQty()
        {
            if (MessageVersion >= 9)
                Order.MinimumQuantity = R.ReadIntNullable();
        }

        internal void readOcaType()
        {
            if (MessageVersion >= 9)
                Order.OcaType = R.ReadEnum<OcaType>();
        }

        internal void skipETradeOnly()
        {
            if (MessageVersion >= 9)
                R.ReadBool();
        }

        internal void skipFirmQuoteOnly()
        {
            if (MessageVersion >= 9)
                R.ReadBool();
        }

        internal void skipNbboPriceCap()
        {
            if (MessageVersion >= 9)
                R.ReadDoubleNullable();
        }

        internal void readParentId()
        {
            if (MessageVersion >= 10)
                Order.ParentId = R.ReadInt();
        }

        internal void readTriggerMethod()
        {
            if (MessageVersion >= 10)
                Order.TriggerMethod = R.ReadEnum<TriggerMethod>();
        }

        internal void readVolOrderParams(bool readOpenOrderAttribs)
        {
            if (MessageVersion >= 11)
            {
                Order.Volatility = R.ReadDoubleNullable();
                Order.VolatilityType = R.ReadEnum<VolatilityType>();
                if (MessageVersion == 11)
                    Order.DeltaNeutralOrderType = ((R.ReadInt() == 0) ? "NONE" : "MKT");
                else
                { // msgVersion 12 and up
                    Order.DeltaNeutralOrderType = R.ReadString();
                    Order.DeltaNeutralAuxPrice = R.ReadDoubleNullable();

                    if (MessageVersion >= 27 && Order.DeltaNeutralOrderType != "")
                    {
                        Order.DeltaNeutralContractId = R.ReadInt();
                        if (readOpenOrderAttribs)
                        {
                            Order.DeltaNeutralSettlingFirm = R.ReadString();
                            Order.DeltaNeutralClearingAccount = R.ReadString();
                            Order.DeltaNeutralClearingIntent = R.ReadString();
                        }
                    }

                    if (MessageVersion >= 31 && Order.DeltaNeutralOrderType != "")
                    {
                        if (readOpenOrderAttribs)
                            Order.DeltaNeutralOpenClose = R.ReadString();
                        Order.DeltaNeutralShortSale = R.ReadBool();
                        Order.DeltaNeutralShortSaleSlot = R.ReadInt();
                        Order.DeltaNeutralDesignatedLocation = R.ReadString();
                    }
                }
                Order.ContinuousUpdate = R.ReadInt();
                if ((int)R.Config.ServerVersionCurrent == 26)
                {
                    Order.StockRangeLower = R.ReadDouble();
                    Order.StockRangeUpper = R.ReadDouble();
                }
                Order.ReferencePriceType = R.ReadEnum<ReferencePriceType>();
            }
        }

        internal void readTrailParams()
        {
            if (MessageVersion >= 13)
                Order.TrailingStopPrice = R.ReadDoubleNullable();
            if (MessageVersion >= 30)
                Order.TrailingStopPercent = R.ReadDoubleNullable();
        }

        internal void readBasisPoints()
        {
            if (MessageVersion >= 14)
            {
                Order.BasisPoints = R.ReadDoubleNullable();
                Order.BasisPointsType = R.ReadIntNullable();
            }
        }

        internal void readComboLegs()
        {
            if (MessageVersion >= 14)
                Contract.ComboLegsDescription = R.ReadString();

            if (MessageVersion >= 29)
            {
                int n = R.ReadInt();
                if (n > 0)
                {
                    for (int i = 0; i < n; ++i)
                        Contract.ComboLegs.Add(new ContractComboLeg(R));
                }

                n = R.ReadInt();
                if (n > 0)
                {
                    for (int i = 0; i < n; ++i)
                        Order.ComboLegs.Add(new OrderComboLeg(R.ReadDoubleNullable()));
                }
            }
        }

        internal void readSmartComboRoutingParams()
        {
            if (MessageVersion >= 26)
            {
                int n = R.ReadInt();
                if (n > 0)
                {
                    for (int i = 0; i < n; ++i)
                        Order.SmartComboRoutingParams.Add(new Tag(R));
                }
            }
        }

        internal void readScaleOrderParams()
        {
            if (MessageVersion >= 15)
            {
                if (MessageVersion >= 20)
                {
                    Order.ScaleInitLevelSize = R.ReadIntNullable();
                    Order.ScaleSubsLevelSize = R.ReadIntNullable();
                }
                else
                {
                    // int notSuppScaleNumComponents
                    R.ReadIntNullable();
                    Order.ScaleInitLevelSize = R.ReadIntNullable();
                }
                Order.ScalePriceIncrement = R.ReadDoubleNullable();
            }

            if (MessageVersion >= 28 && Order.ScalePriceIncrement > 0.0 && Order.ScalePriceIncrement != double.MaxValue)
            {
                Order.ScalePriceAdjustValue = R.ReadDoubleNullable();
                Order.ScalePriceAdjustInterval = R.ReadIntNullable();
                Order.ScaleProfitOffset = R.ReadDoubleNullable();
                Order.ScaleAutoReset = R.ReadBool();
                Order.ScaleInitPosition = R.ReadIntNullable();
                Order.ScaleInitFillQty = R.ReadIntNullable();
                Order.ScaleRandomPercent = R.ReadBool();
            }
        }

        internal void readHedgeParams()
        {
            if (MessageVersion >= 24)
            {
                Order.HedgeType = R.ReadStringEnum<HedgeType>();
                if (Order.HedgeType != HedgeType.Undefined)
                    Order.HedgeParam = R.ReadString();
            }
        }

        internal void readOptOutSmartRouting()
        {
            if (MessageVersion >= 25)
                Order.OptOutSmartRouting = R.ReadBool();
        }

        internal void readClearingParams()
        {
            if (MessageVersion >= 19)
            {
                Order.ClearingAccount = R.ReadString();
                Order.ClearingIntent = R.ReadStringEnum<ClearingIntent>();
            }
        }

        internal void readNotHeld()
        {
            if (MessageVersion >= 22)
                Order.NotHeld = R.ReadBool();
        }

        internal void readDeltaNeutral()
        {
            if (MessageVersion >= 20)
            {
                if (R.ReadBool())
                    Contract.DeltaNeutralContract = new DeltaNeutralContract(R, false);
            }
        }

        internal void readAlgoParams()
        {
            if (MessageVersion >= 21)
            {
                Order.AlgoStrategy = R.ReadString();
                if (Order.AlgoStrategy != "")
                {
                    int n = R.ReadInt();
                    if (n > 0)
                    {
                        for (int i = 0; i < n; ++i)
                            Order.AlgoParams.Add(new Tag(R));
                    }
                }
            }
        }

        internal void readSolicited()
        {
            if (MessageVersion >= 33)
                Order.Solicited = R.ReadBool();
        }

        internal void readWhatIfInfoAndCommission()
        {
            if (MessageVersion >= 16)
            {
                Order.WhatIf = R.ReadBool();
                readOrderStatus();
                if (R.Config.SupportsServerVersion(ServerVersion.WHAT_IF_EXT_FIELDS))
                {
                    OrderState.InitialMarginBefore = R.ReadString();
                    OrderState.MaintenanceMarginBefore = R.ReadString();
                    OrderState.EquityWithLoanBefore = R.ReadString();
                    OrderState.InitMarginChange = R.ReadString();
                    OrderState.MaintMarginChange = R.ReadString();
                    OrderState.EquityWithLoanChange = R.ReadString();
                }
                OrderState.InitMarginAfter = R.ReadString();
                OrderState.MaintMarginAfter = R.ReadString();
                OrderState.EquityWithLoanAfter = R.ReadString();
                OrderState.Commission = R.ReadDoubleNullable();
                OrderState.MinimumCommission = R.ReadDoubleNullable();
                OrderState.MaximumCommission = R.ReadDoubleNullable();
                OrderState.CommissionCurrency = R.ReadString();
                OrderState.WarningText = R.ReadString();
            }

        }

        internal void readOrderStatus() => OrderState.Status = R.ReadStringEnum<OrderStatus>();

        internal void readVolRandomizeFlags()
        {
            if (MessageVersion >= 34)
            {
                Order.RandomizeSize = R.ReadBool();
                Order.RandomizePrice = R.ReadBool();
            }
        }

        internal void readPegToBenchParams()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
            {
                var type = R.ReadStringEnum<OrderType>();
                if (type == OrderType.PeggedToBenchmark)
                {
                    Order.ReferenceContractId = R.ReadInt();
                    Order.IsPeggedChangeAmountDecrease = R.ReadBool();
                    Order.PeggedChangeAmount = R.ReadDoubleNullable();
                    Order.ReferenceChangeAmount = R.ReadDoubleNullable();
                    Order.ReferenceExchange = R.ReadString();
                }
            }
        }

        internal void readConditions()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
            {
                //string s = R.ReadString();
                int n = R.ReadInt();
                //int n = 0;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        var orderConditionType = R.ReadEnum<OrderConditionType>();
                        OrderCondition condition = OrderCondition.Create(orderConditionType);
                        condition.Deserialize(R);
                        Order.Conditions.Add(condition);
                    }
                    Order.ConditionsIgnoreRegularTradingHours = R.ReadBool();
                    Order.ConditionsCancelOrder = R.ReadBool();
                }
            }

        }

        internal void readAdjustedOrderParams()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
            {
                Order.AdjustedOrderType = R.ReadString();
                Order.TriggerPrice = R.ReadDoubleNullable();
                readStopPriceAndLmtPriceOffset();
                Order.AdjustedStopPrice = R.ReadDoubleNullable();
                Order.AdjustedStopLimitPrice = R.ReadDoubleNullable();
                Order.AdjustedTrailingAmount = R.ReadDoubleNullable();
                Order.AdjustableTrailingUnit = R.ReadInt();
            }
        }

        internal void readStopPriceAndLmtPriceOffset()
        {
            Order.TrailingStopPrice = R.ReadDoubleNullable();
            Order.LmtPriceOffset = R.ReadDoubleNullable();
        }

        internal void readSoftDollarTier()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.SOFT_DOLLAR_TIER))
                Order.SoftDollarTier.Set(R);
        }

        internal void readCashQty()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.CASH_QTY))
                Order.CashQty = R.ReadDoubleNullable();
        }

        internal void readDontUseAutoPriceForHedge()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.AUTO_PRICE_FOR_HEDGE))
                Order.DontUseAutoPriceForHedge = R.ReadBool();
        }

        internal void readIsOmsContainer()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
                Order.IsOmsContainer = R.ReadBool();
        }

        internal void readDiscretionaryUpToLimitPrice()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.D_PEG_ORDERS))
                Order.DiscretionaryUpToLimitPrice = R.ReadBool();
        }

        internal void readAutoCancelDate() => Order.AutoCancelDate = R.ReadString();
        internal void readFilledQuantity() => Order.FilledQuantity = R.ReadDoubleNullable();
        internal void readRefFuturesConId() => Order.RefFuturesConId = R.ReadInt();
        internal void readAutoCancelParent() => Order.AutoCancelParent = R.ReadBool();
        internal void readShareholder() => Order.Shareholder = R.ReadString();
        internal void readImbalanceOnly() => Order.ImbalanceOnly = R.ReadBool();
        internal void readRouteMarketableToBbo() => Order.RouteMarketableToBbo = R.ReadBool();
        internal void readParentPermId() => Order.ParentPermId = R.ReadLong();
        internal void readCompletedTime() => OrderState.CompletedTime = R.ReadString();
        internal void readCompletedStatus() => OrderState.CompletedStatus = R.ReadString();

        internal void readUsePriceMgmtAlgo()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.PRICE_MGMT_ALGO))
                Order.UsePriceMgmtAlgo = R.ReadBool();
        }

        internal void readDuration()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.DURATION))
                Order.Duration = R.ReadIntNullable();
        }

        internal void readPostToAts()
        {
            if (R.Config.SupportsServerVersion(ServerVersion.POST_TO_ATS))
                Order.PostToAts = R.ReadIntNullable();
        }
    }
}
