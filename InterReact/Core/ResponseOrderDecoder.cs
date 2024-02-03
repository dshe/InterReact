namespace InterReact;

internal sealed class OrderDecoder
{
    private ResponseReader R { get; }
    private Contract Contract { get; }
    private Order Order { get; }
    private OrderState OrderState { get; }

    internal OrderDecoder(ResponseReader reader, Contract contract, Order order, OrderState orderState)
    {
        R = reader;
        Contract = contract;
        Order = order;
        OrderState = orderState;
    }

    internal void ReadOrderId() => Order.OrderId = R.ReadInt();
    internal void ReadAction() => Order.Action = R.ReadString();

    internal void ReadContract()
    {
        Contract.ContractId = R.ReadInt();
        Contract.Symbol = R.ReadString();
        Contract.SecurityType = R.ReadString();
        Contract.LastTradeDateOrContractMonth = R.ReadString();
        Contract.Strike = R.ReadDouble();
        Contract.Right = R.ReadString();
        Contract.Multiplier = R.ReadString();
        Contract.Exchange = R.ReadString();
        Contract.Currency = R.ReadString();
        Contract.LocalSymbol = R.ReadString();
        Contract.TradingClass = R.ReadString();
    }

    internal void ReadTotalQuantity() => Order.TotalQuantity = R.ReadDecimal();
    internal void ReadOrderType() => Order.OrderType = R.ReadString();
    internal void ReadLmtPrice() => Order.LimitPrice = R.ReadDoubleMax();
    internal void ReadAuxPrice() => Order.AuxPrice = R.ReadDoubleMax();
    internal void ReadTif() => Order.TimeInForce = R.ReadString();
    internal void ReadOcaGroup() => Order.OcaGroup = R.ReadString();
    internal void ReadAccount() => Order.Account = R.ReadString();
    internal void ReadOpenClose() => Order.OpenClose = R.ReadString();
    internal void ReadOrigin() => Order.Origin = R.ReadEnum<OrderOrigin>();
    internal void ReadOrderRef() => Order.OrderRef = R.ReadString();
    internal void ReadClientId() => Order.ClientId = R.ReadInt();
    internal void ReadPermId() => Order.PermanentId = R.ReadInt();
    internal void ReadOutsideRth() => Order.OutsideRegularTradingHours = R.ReadBool();
    internal void ReadHidden() => Order.Hidden = R.ReadInt() == 1;
    internal void ReadDiscretionaryAmount() => Order.DiscretionaryAmount = R.ReadDouble();
    internal void ReadGoodAfterTime() => Order.GoodAfterTime = R.ReadString();
    internal void SkipSharesAllocation() => R.ReadString(); // skip deprecated sharesAllocation field

    internal void ReadFaParams()
    {
        Order.FinancialAdvisorGroup = R.ReadString();
        Order.FinancialAdvisorMethod = R.ReadString();
        Order.FinancialAdvisorPercentage = R.ReadString();
        Order.FinancialAdvisorProfile = R.ReadString();
    }

    internal void ReadModelCode() => Order.ModelCode = R.ReadString();
    internal void ReadGoodTillDate() => Order.GoodUntilDate = R.ReadString();
    internal void ReadRule80A() => Order.Rule80A = R.ReadString();
    internal void ReadPercentOffset() => Order.PercentOffset = R.ReadDoubleMax();
    internal void ReadSettlingFirm() => Order.SettlingFirm = R.ReadString();

    internal void ReadShortSaleParams()
    {
        Order.ShortSaleSlot = R.ReadEnum<ShortSaleSlot>();
        Order.DesignatedLocation = R.ReadString();
        Order.ExemptCode = R.ReadInt();
    }

    internal void ReadAuctionStrategy() => Order.AuctionStrategy = R.ReadEnum<AuctionStrategy>();

    internal void ReadBoxOrderParams()
    {
        Order.StartingPrice = R.ReadDoubleMax();
        Order.StockReferencePrice = R.ReadDoubleMax();
        Order.Delta = R.ReadDoubleMax();
    }

    internal void ReadPegToStkOrVolOrderParams()
    {
        Order.StockRangeLower = R.ReadDoubleMax();
        Order.StockRangeUpper = R.ReadDoubleMax();
    }

    internal void ReadDisplaySize() => Order.DisplaySize = R.ReadIntMax();
    internal static void ReadOldStyleOutsideRth() { }

    internal void ReadBlockOrder() => Order.BlockOrder = R.ReadBool();
    internal void ReadSweepToFill() => Order.SweepToFill = R.ReadBool();
    internal void ReadAllOrNone() => Order.AllOrNone = R.ReadBool();

    internal void ReadMinQty() => Order.MinQty = R.ReadIntMax();
    internal void ReadOcaType() => Order.OcaType = R.ReadEnum<OcaType>();
    internal void SkipETradeOnly() => R.ReadBool();
    internal void SkipFirmQuoteOnly() => R.ReadBool();
    internal void SkipNbboPriceCap() => R.ReadDoubleMax();
    internal void ReadParentId() => Order.ParentId = R.ReadInt();
    internal void ReadTriggerMethod() => Order.TriggerMethod = R.ReadEnum<TriggerMethod>();

    internal void ReadVolOrderParams(bool readOpenOrderAttribs)
    {
        Order.Volatility = R.ReadDoubleMax();
        Order.VolatilityType = R.ReadEnum<VolatilityType>();
        Order.DeltaNeutralOrderType = R.ReadString();
        Order.DeltaNeutralAuxPrice = R.ReadDoubleMax();

        if (Order.DeltaNeutralOrderType.Length != 0)
        {
            Order.DeltaNeutralContractId = R.ReadInt();
            if (readOpenOrderAttribs)
            {
                Order.DeltaNeutralSettlingFirm = R.ReadString();
                Order.DeltaNeutralClearingAccount = R.ReadString();
                Order.DeltaNeutralClearingIntent = R.ReadString();
                Order.DeltaNeutralOpenClose = R.ReadString();
            }
            Order.DeltaNeutralShortSale = R.ReadBool();
            Order.DeltaNeutralShortSaleSlot = R.ReadInt();
            Order.DeltaNeutralDesignatedLocation = R.ReadString();
        }

        Order.ContinuousUpdate = R.ReadInt();
        Order.ReferencePriceType = R.ReadEnum<ReferencePriceType>();
    }

    internal void ReadTrailParams()
    {
        Order.TrailingStopPrice = R.ReadDoubleMax();
        Order.TrailingStopPercent = R.ReadDoubleMax();
    }

    internal void ReadBasisPoints()
    {
        Order.BasisPoints = R.ReadDoubleMax();
        Order.BasisPointsType = R.ReadIntMax();
    }

    internal void ReadComboLegs()
    {
        Contract.ComboLegsDescription = R.ReadString();

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
                Order.OrderComboLegs.Add(new OrderComboLeg(R.ReadDoubleMax()));
        }
    }

    internal void ReadSmartComboRoutingParams()
    {
        int n = R.ReadInt();
        if (n > 0)
        {
            for (int i = 0; i < n; ++i)
                Order.SmartComboRoutingParams.Add(new Tag(R));
        }
    }

    internal void ReadScaleOrderParams()
    {
        Order.ScaleInitLevelSize = R.ReadIntMax();
        Order.ScaleSubsLevelSize = R.ReadIntMax();
        Order.ScalePriceIncrement = R.ReadDoubleMax();

        if (Order.ScalePriceIncrement > 0.0 && Order.ScalePriceIncrement != double.MaxValue)
        {
            Order.ScalePriceAdjustValue = R.ReadDoubleMax();
            Order.ScalePriceAdjustInterval = R.ReadIntMax();
            Order.ScaleProfitOffset = R.ReadDoubleMax();
            Order.ScaleAutoReset = R.ReadBool();
            Order.ScaleInitPosition = R.ReadIntMax();
            Order.ScaleInitFillQty = R.ReadIntMax();
            Order.ScaleRandomPercent = R.ReadBool();
        }
    }

    internal void ReadHedgeParams()
    {
        Order.HedgeType = R.ReadString();
        if (Order.HedgeType.Length != 0)
            Order.HedgeParam = R.ReadString();
    }

    internal void ReadOptOutSmartRouting() => Order.OptOutSmartRouting = R.ReadBool();

    internal void ReadClearingParams()
    {
        Order.ClearingAccount = R.ReadString();
        Order.ClearingIntent = R.ReadString();
    }

    internal void ReadNotHeld() => Order.NotHeld = R.ReadBool();

    internal void ReadDeltaNeutral()
    {
        if (R.ReadBool())
            Contract.DeltaNeutralContract = new DeltaNeutralContract(R, false);
    }

    internal void ReadAlgoParams()
    {
        Order.AlgoStrategy = R.ReadString();
        if (Order.AlgoStrategy.Length != 0)
        {
            int n = R.ReadInt();
            if (n > 0)
            {
                for (int i = 0; i < n; ++i)
                    Order.AlgoParams.Add(new Tag(R));
            }
        }
    }

    internal void ReadSolicited() => Order.Solicited = R.ReadBool();

    internal void ReadWhatIfInfoAndCommission()
    {
        Order.WhatIf = R.ReadBool();
        ReadOrderStatus();

        OrderState.InitialMarginBefore = R.ReadString();
        OrderState.MaintenanceMarginBefore = R.ReadString();
        OrderState.EquityWithLoanBefore = R.ReadString();
        OrderState.InitMarginChange = R.ReadString();
        OrderState.MaintMarginChange = R.ReadString();
        OrderState.EquityWithLoanChange = R.ReadString();

        OrderState.InitMarginAfter = R.ReadString();
        OrderState.MaintMarginAfter = R.ReadString();
        OrderState.EquityWithLoanAfter = R.ReadString();
        OrderState.Commission = R.ReadDoubleMax();
        OrderState.MinimumCommission = R.ReadDoubleMax();
        OrderState.MaximumCommission = R.ReadDoubleMax();
        OrderState.CommissionCurrency = R.ReadString();
        OrderState.WarningText = R.ReadString();
    }

    internal void ReadOrderStatus() => OrderState.Status = R.ReadString();

    internal void ReadVolRandomizeFlags()
    {
        Order.RandomizeSize = R.ReadBool();
        Order.RandomizePrice = R.ReadBool();
    }

    internal void ReadPegToBenchParams()
    {
        if (Order.OrderType == OrderTypes.PeggedToBenchmark)
        {
            Order.ReferenceContractId = R.ReadInt();
            Order.IsPeggedChangeAmountDecrease = R.ReadBool();
            Order.PeggedChangeAmount = R.ReadDoubleMax();
            Order.ReferenceChangeAmount = R.ReadDoubleMax();
            Order.ReferenceExchange = R.ReadString();
        }
    }

    internal void ReadConditions()
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

    internal void ReadAdjustedOrderParams()
    {
        Order.AdjustedOrderType = R.ReadString();
        Order.TriggerPrice = R.ReadDoubleMax();
        ReadStopPriceAndLmtPriceOffset();
        Order.AdjustedStopPrice = R.ReadDoubleMax();
        Order.AdjustedStopLimitPrice = R.ReadDoubleMax();
        Order.AdjustedTrailingAmount = R.ReadDoubleMax();
        Order.AdjustableTrailingUnit = R.ReadInt();
    }

    internal void ReadStopPriceAndLmtPriceOffset()
    {
        Order.TrailingStopPrice = R.ReadDoubleMax();
        Order.LmtPriceOffset = R.ReadDoubleMax();
    }

    internal void ReadSoftDollarTier() => Order.SoftDollarTier.Set(R);
    internal void ReadCashQty() => Order.CashQty = R.ReadDoubleMax();
    internal void ReadDontUseAutoPriceForHedge() => Order.DontUseAutoPriceForHedge = R.ReadBool();
    internal void ReadIsOmsContainer() => Order.IsOmsContainer = R.ReadBool();

    internal void ReadDiscretionaryUpToLimitPrice() => Order.DiscretionaryUpToLimitPrice = R.ReadBool();
    internal void ReadAutoCancelDate() => Order.AutoCancelDate = R.ReadString();
    internal void ReadFilledQuantity() => Order.FilledQuantity = R.ReadDecimal();
    internal void ReadRefFuturesConId() => Order.RefFuturesConId = R.ReadInt();

    internal void ReadAutoCancelParent() => Order.AutoCancelParent = R.ReadBool();
    internal void ReadShareholder() => Order.Shareholder = R.ReadString();
    internal void ReadImbalanceOnly() => Order.ImbalanceOnly = R.ReadBool();
    internal void ReadRouteMarketableToBbo() => Order.RouteMarketableToBbo = R.ReadBool();

    internal void ReadParentPermId() => Order.ParentPermId = R.ReadLong();
    internal void ReadCompletedTime() => OrderState.CompletedTime = R.ReadString();
    internal void ReadCompletedStatus() => OrderState.CompletedStatus = R.ReadString();
    internal void ReadUsePriceMgmtAlgo() => Order.UsePriceMgmtAlgo = R.ReadBool();

    internal void ReadDuration() => Order.Duration = R.ReadIntMax();
    internal void ReadPostToAts() => Order.PostToAts = R.ReadIntMax();

    internal void ReadPegBestPegMidOrderAttributes()
    {
        Order.MinTradeQty = R.ReadIntMax();
        Order.MinCompeteSize = R.ReadIntMax();
        Order.CompeteAgainstBestOffset = R.ReadDoubleMax();
        Order.MidOffsetAtWhole = R.ReadDoubleMax();
        Order.MidOffsetAtHalf = R.ReadDoubleMax();
    }
}
