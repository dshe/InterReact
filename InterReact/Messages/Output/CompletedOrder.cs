namespace InterReact
{
    public sealed class CompletedOrder // does not have OrderId!
    {
        public Contract Contract { get; } = new Contract();
        public Order Order { get; } = new Order();
        public OrderState OrderState { get; } = new OrderState();

        internal CompletedOrder() { }
        internal CompletedOrder(ResponseReader reader)
        {
            OrderDecoder decoder = new(reader, Contract, Order, OrderState, int.MaxValue);

            decoder.ReadContract();
            decoder.ReadAction();
            decoder.ReadTotalQuantity();
            decoder.ReadOrderType();
            decoder.ReadLmtPrice();
            decoder.ReadAuxPrice();
            decoder.ReadTIF();
            decoder.ReadOcaGroup();
            decoder.ReadAccount();
            decoder.ReadOpenClose();
            decoder.ReadOrigin();
            decoder.ReadOrderRef();
            decoder.ReadPermId();
            decoder.ReadOutsideRth();
            decoder.ReadHidden();
            decoder.ReadDiscretionaryAmount();
            decoder.ReadGoodAfterTime();
            decoder.ReadFAParams();
            decoder.ReadModelCode();
            decoder.ReadGoodTillDate();
            decoder.ReadRule80A();
            decoder.ReadPercentOffset();
            decoder.ReadSettlingFirm();
            decoder.ReadShortSaleParams();
            decoder.ReadBoxOrderParams();
            decoder.ReadPegToStkOrVolOrderParams();
            decoder.ReadDisplaySize();
            decoder.ReadSweepToFill();
            decoder.ReadAllOrNone();
            decoder.ReadMinQty();
            decoder.ReadOcaType();
            decoder.ReadTriggerMethod();
            decoder.ReadVolOrderParams(false);
            decoder.ReadTrailParams();
            decoder.ReadComboLegs();
            decoder.ReadSmartComboRoutingParams();
            decoder.ReadScaleOrderParams();
            decoder.ReadHedgeParams();
            decoder.ReadClearingParams();
            decoder.ReadNotHeld();
            decoder.ReadDeltaNeutral();
            decoder.ReadAlgoParams();
            decoder.ReadSolicited();
            decoder.ReadOrderStatus();
            decoder.ReadVolRandomizeFlags();
            decoder.ReadPegToBenchParams();
            decoder.ReadConditions();
            decoder.ReadStopPriceAndLmtPriceOffset();
            decoder.ReadCashQty();
            decoder.ReadDontUseAutoPriceForHedge();
            decoder.ReadIsOmsContainer();
            decoder.ReadAutoCancelDate();
            decoder.ReadFilledQuantity();
            decoder.ReadRefFuturesConId();
            decoder.ReadAutoCancelParent();
            decoder.ReadShareholder();
            decoder.ReadImbalanceOnly();
            decoder.ReadRouteMarketableToBbo();
            decoder.ReadParentPermId();
            decoder.ReadCompletedTime();
            decoder.ReadCompletedStatus();
        }
    }

    public sealed class CompletedOrdersEnd { }
}