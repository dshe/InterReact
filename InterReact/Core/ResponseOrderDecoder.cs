namespace InterReact;

internal sealed class OrderDecoder
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

    internal void ReadOrderId() => Order.OrderId = R.ReadInt();
    internal void ReadAction() => Order.OrderAction = R.ReadStringEnum<OrderAction>();

    public void ReadContract()
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

    internal void ReadTotalQuantity() => Order.TotalQuantity = R.ReadDouble();
    internal void ReadOrderType() => Order.OrderType = R.ReadStringEnum<OrderType>();
    internal void ReadLmtPrice() => Order.LimitPrice = (MessageVersion < 29) ? R.ReadDouble() : R.ReadDoubleNullable();
    internal void ReadAuxPrice() => Order.AuxPrice = (MessageVersion < 30) ? R.ReadDouble() : R.ReadDoubleNullable();
    internal void ReadTIF() => Order.TimeInForce = R.ReadStringEnum<TimeInForce>();
    internal void ReadOcaGroup() => Order.OcaGroup = R.ReadString();
    internal void ReadAccount() => Order.Account = R.ReadString();
    internal void ReadOpenClose() => Order.OpenClose = R.ReadStringEnum<OrderOpenClose>();
    internal void ReadOrigin() => Order.Origin = R.ReadEnum<OrderOrigin>();
    internal void ReadOrderRef() => Order.OrderRef = R.ReadString();

    internal void ReadClientId()
    {
        if (MessageVersion >= 3)
            Order.ClientId = R.ReadInt();
    }

    internal void ReadPermId()
    {
        if (MessageVersion >= 4)
            Order.PermanentId = R.ReadInt();
    }

    internal void ReadOutsideRth()
    {
        if (MessageVersion >= 4)
        {
            if (MessageVersion < 18)
                R.ReadString(); // will never happen, order.ignoreRth
            else
                Order.OutsideRegularTradingHours = R.ReadBool();
        }
    }

    internal void ReadHidden()
    {
        if (MessageVersion >= 4)
            Order.Hidden = R.ReadInt() == 1;
    }

    internal void ReadDiscretionaryAmount()
    {
        if (MessageVersion >= 4)
            Order.DiscretionaryAmount = R.ReadDouble();
    }

    internal void ReadGoodAfterTime()
    {
        if (MessageVersion >= 5)
            Order.GoodAfterTime = R.ReadString();
    }

    internal void SkipSharesAllocation()
    {
        if (MessageVersion >= 6)
            R.ReadString(); // skip deprecated sharesAllocation field
    }

    internal void ReadFAParams()
    {
        if (MessageVersion >= 7)
        {
            Order.FinancialAdvisorGroup = R.ReadString();
            Order.FinancialAdvisorMethod = R.ReadStringEnum<FinancialAdvisorAllocationMethod>();
            Order.FinancialAdvisorPercentage = R.ReadString();
            Order.FinancialAdvisorProfile = R.ReadString();
        }
    }

    internal void ReadModelCode()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.MODELS_SUPPORT))
            Order.ModelCode = R.ReadString();
    }

    internal void ReadGoodTillDate()
    {
        if (MessageVersion >= 8)
            Order.GoodUntilDate = R.ReadString();
    }

    internal void ReadRule80A()
    {
        if (MessageVersion >= 9)
            Order.Rule80A = R.ReadStringEnum<AgentDescription>();
    }

    internal void ReadPercentOffset()
    {
        if (MessageVersion >= 9)
            Order.PercentOffset = R.ReadDoubleNullable();
    }

    internal void ReadSettlingFirm()
    {
        if (MessageVersion >= 9)
            Order.SettlingFirm = R.ReadString();
    }

    internal void ReadShortSaleParams()
    {
        if (MessageVersion >= 9)
        {
            Order.ShortSaleSlot = R.ReadEnum<ShortSaleSlot>();
            Order.DesignatedLocation = R.ReadString();
            if ((int)R.Connector.ServerVersionCurrent == 51)
                R.ReadInt(); // exemptCode
            else if (MessageVersion >= 23)
                Order.ExemptCode = R.ReadInt();
        }
    }

    internal void ReadAuctionStrategy()
    {
        if (MessageVersion >= 9)
            Order.AuctionStrategy = R.ReadEnum<AuctionStrategy>();
    }

    internal void ReadBoxOrderParams()
    {
        if (MessageVersion >= 9)
        {
            Order.StartingPrice = R.ReadDoubleNullable();
            Order.StockReferencePrice = R.ReadDoubleNullable();
            Order.Delta = R.ReadDoubleNullable();
        }
    }

    internal void ReadPegToStkOrVolOrderParams()
    {
        if (MessageVersion >= 9)
        {
            Order.StockRangeLower = R.ReadDoubleNullable();
            Order.StockRangeUpper = R.ReadDoubleNullable();
        }
    }

    internal void ReadDisplaySize()
    {
        if (MessageVersion >= 9)
            Order.DisplaySize = R.ReadIntNullable();
    }

    internal void ReadOldStyleOutsideRth()
    {
        if (MessageVersion >= 9 && MessageVersion < 18)
            R.ReadBool(); // will never happen order.rthOnly
    }

    internal void ReadBlockOrder()
    {
        if (MessageVersion >= 9)
            Order.BlockOrder = R.ReadBool();
    }

    internal void ReadSweepToFill()
    {
        if (MessageVersion >= 9)
            Order.SweepToFill = R.ReadBool();
    }

    internal void ReadAllOrNone()
    {
        if (MessageVersion >= 9)
            Order.AllOrNone = R.ReadBool();
    }

    internal void ReadMinQty()
    {
        if (MessageVersion >= 9)
            Order.MinimumQuantity = R.ReadIntNullable();
    }

    internal void ReadOcaType()
    {
        if (MessageVersion >= 9)
            Order.OcaType = R.ReadEnum<OcaType>();
    }

    internal void SkipETradeOnly()
    {
        if (MessageVersion >= 9)
            R.ReadBool();
    }

    internal void SkipFirmQuoteOnly()
    {
        if (MessageVersion >= 9)
            R.ReadBool();
    }

    internal void SkipNbboPriceCap()
    {
        if (MessageVersion >= 9)
            R.ReadDoubleNullable();
    }

    internal void ReadParentId()
    {
        if (MessageVersion >= 10)
            Order.ParentId = R.ReadInt();
    }

    internal void ReadTriggerMethod()
    {
        if (MessageVersion >= 10)
            Order.TriggerMethod = R.ReadEnum<TriggerMethod>();
    }

    internal void ReadVolOrderParams(bool readOpenOrderAttribs)
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

                if (MessageVersion >= 27 && Order.DeltaNeutralOrderType.Length != 0)
                {
                    Order.DeltaNeutralContractId = R.ReadInt();
                    if (readOpenOrderAttribs)
                    {
                        Order.DeltaNeutralSettlingFirm = R.ReadString();
                        Order.DeltaNeutralClearingAccount = R.ReadString();
                        Order.DeltaNeutralClearingIntent = R.ReadString();
                    }
                }

                if (MessageVersion >= 31 && Order.DeltaNeutralOrderType.Length != 0)
                {
                    if (readOpenOrderAttribs)
                        Order.DeltaNeutralOpenClose = R.ReadString();
                    Order.DeltaNeutralShortSale = R.ReadBool();
                    Order.DeltaNeutralShortSaleSlot = R.ReadInt();
                    Order.DeltaNeutralDesignatedLocation = R.ReadString();
                }
            }
            Order.ContinuousUpdate = R.ReadInt();
            if ((int)R.Connector.ServerVersionCurrent == 26)
            {
                Order.StockRangeLower = R.ReadDouble();
                Order.StockRangeUpper = R.ReadDouble();
            }
            Order.ReferencePriceType = R.ReadEnum<ReferencePriceType>();
        }
    }

    internal void ReadTrailParams()
    {
        if (MessageVersion >= 13)
            Order.TrailingStopPrice = R.ReadDoubleNullable();
        if (MessageVersion >= 30)
            Order.TrailingStopPercent = R.ReadDoubleNullable();
    }

    internal void ReadBasisPoints()
    {
        if (MessageVersion >= 14)
        {
            Order.BasisPoints = R.ReadDoubleNullable();
            Order.BasisPointsType = R.ReadIntNullable();
        }
    }

    internal void ReadComboLegs()
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

    internal void ReadSmartComboRoutingParams()
    {
        if (MessageVersion >= 26)
        {
            int n = R.ReadInt();
            if (n > 0)
            {
                for (int i = 0; i < n; ++i)
                    Order.SmartComboRoutingParams.Add(new Tag(R.ReadString(), R.ReadString()));
            }
        }
    }

    internal void ReadScaleOrderParams()
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

    internal void ReadHedgeParams()
    {
        if (MessageVersion >= 24)
        {
            Order.HedgeType = R.ReadStringEnum<HedgeType>();
            if (Order.HedgeType != HedgeType.Undefined)
                Order.HedgeParam = R.ReadString();
        }
    }

    internal void ReadOptOutSmartRouting()
    {
        if (MessageVersion >= 25)
            Order.OptOutSmartRouting = R.ReadBool();
    }

    internal void ReadClearingParams()
    {
        if (MessageVersion >= 19)
        {
            Order.ClearingAccount = R.ReadString();
            Order.ClearingIntent = R.ReadStringEnum<ClearingIntent>();
        }
    }

    internal void ReadNotHeld()
    {
        if (MessageVersion >= 22)
            Order.NotHeld = R.ReadBool();
    }

    internal void ReadDeltaNeutral()
    {
        if (MessageVersion >= 20)
        {
            if (R.ReadBool())
                Contract.DeltaNeutralContract = new DeltaNeutralContract(R, false);
        }
    }

    internal void ReadAlgoParams()
    {
        if (MessageVersion >= 21)
        {
            Order.AlgoStrategy = R.ReadString();
            if (Order.AlgoStrategy.Length != 0)
            {
                int n = R.ReadInt();
                if (n > 0)
                {
                    for (int i = 0; i < n; ++i)
                        Order.AlgoParams.Add(new Tag(R.ReadString(), R.ReadString()));
                }
            }
        }
    }

    internal void ReadSolicited()
    {
        if (MessageVersion >= 33)
            Order.Solicited = R.ReadBool();
    }

    internal void ReadWhatIfInfoAndCommission()
    {
        if (MessageVersion >= 16)
        {
            Order.WhatIf = R.ReadBool();
            ReadOrderStatus();
            if (R.Connector.SupportsServerVersion(ServerVersion.WHAT_IF_EXT_FIELDS))
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

    internal void ReadOrderStatus() => OrderState.Status = R.ReadStringEnum<OrderStatus>();

    internal void ReadVolRandomizeFlags()
    {
        if (MessageVersion >= 34)
        {
            Order.RandomizeSize = R.ReadBool();
            Order.RandomizePrice = R.ReadBool();
        }
    }

    internal void ReadPegToBenchParams()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
        {
            if (Order.OrderType == OrderType.PeggedToBenchmark)
            {
                Order.ReferenceContractId = R.ReadInt();
                Order.IsPeggedChangeAmountDecrease = R.ReadBool();
                Order.PeggedChangeAmount = R.ReadDoubleNullable();
                Order.ReferenceChangeAmount = R.ReadDoubleNullable();
                Order.ReferenceExchange = R.ReadString();
            }
        }
    }

    internal void ReadConditions()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
        {
            int n = R.ReadInt();
            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    OrderConditionType orderConditionType = R.ReadEnum<OrderConditionType>();
                    OrderCondition condition = OrderCondition.Create(orderConditionType);
                    condition.Deserialize(R);
                    Order.Conditions.Add(condition);
                }
                Order.ConditionsIgnoreRegularTradingHours = R.ReadBool();
                Order.ConditionsCancelOrder = R.ReadBool();
            }
        }

    }

    internal void ReadAdjustedOrderParams()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.PEGGED_TO_BENCHMARK))
        {
            Order.AdjustedOrderType = R.ReadString();
            Order.TriggerPrice = R.ReadDoubleNullable();
            ReadStopPriceAndLmtPriceOffset();
            Order.AdjustedStopPrice = R.ReadDoubleNullable();
            Order.AdjustedStopLimitPrice = R.ReadDoubleNullable();
            Order.AdjustedTrailingAmount = R.ReadDoubleNullable();
            Order.AdjustableTrailingUnit = R.ReadInt();
        }
    }

    internal void ReadStopPriceAndLmtPriceOffset()
    {
        Order.TrailingStopPrice = R.ReadDoubleNullable();
        Order.LmtPriceOffset = R.ReadDoubleNullable();
    }

    internal void ReadSoftDollarTier()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.SOFT_DOLLAR_TIER))
            Order.SoftDollarTier.Set(R);
    }

    internal void ReadCashQty()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.CASH_QTY))
            Order.CashQty = R.ReadDoubleNullable();
    }

    internal void ReadDontUseAutoPriceForHedge()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.AUTO_PRICE_FOR_HEDGE))
            Order.DontUseAutoPriceForHedge = R.ReadBool();
    }

    internal void ReadIsOmsContainer()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.ORDER_CONTAINER))
            Order.IsOmsContainer = R.ReadBool();
    }

    internal void ReadDiscretionaryUpToLimitPrice()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.D_PEG_ORDERS))
            Order.DiscretionaryUpToLimitPrice = R.ReadBool();
    }

    internal void ReadAutoCancelDate() => Order.AutoCancelDate = R.ReadString();
    internal void ReadFilledQuantity() => Order.FilledQuantity = R.ReadDoubleNullable();
    internal void ReadRefFuturesConId() => Order.RefFuturesConId = R.ReadInt();
    internal void ReadAutoCancelParent() => Order.AutoCancelParent = R.ReadBool();
    internal void ReadShareholder() => Order.Shareholder = R.ReadString();
    internal void ReadImbalanceOnly() => Order.ImbalanceOnly = R.ReadBool();
    internal void ReadRouteMarketableToBbo() => Order.RouteMarketableToBbo = R.ReadBool();
    internal void ReadParentPermId() => Order.ParentPermId = R.ReadLong();
    internal void ReadCompletedTime() => OrderState.CompletedTime = R.ReadString();
    internal void ReadCompletedStatus() => OrderState.CompletedStatus = R.ReadString();

    internal void ReadUsePriceMgmtAlgo()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.PRICE_MGMT_ALGO))
            Order.UsePriceMgmtAlgo = R.ReadBool();
    }

    internal void ReadDuration()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.DURATION))
            Order.Duration = R.ReadIntNullable();
    }

    internal void ReadPostToAts()
    {
        if (R.Connector.SupportsServerVersion(ServerVersion.POST_TO_ATS))
            Order.PostToAts = R.ReadIntNullable();
    }
}
