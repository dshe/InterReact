namespace InterReact
{
    public sealed class CompletedOrder // does not have OrderId!
    {
        public Contract Contract { get; } = new Contract();
        public Order Order { get; } = new Order();
        public OrderState OrderState { get; } = new OrderState();

        public CompletedOrder(ResponseReader reader)
        {
            OrderDecoder decoder = new(reader, Contract, Order, OrderState, int.MaxValue);

            decoder.readContract();
            decoder.readAction();
            decoder.readTotalQuantity();
            decoder.readOrderType();
            decoder.readLmtPrice();
            decoder.readAuxPrice();
            decoder.readTIF();
            decoder.readOcaGroup();
            decoder.readAccount();
            decoder.readOpenClose();
            decoder.readOrigin();
            decoder.readOrderRef();
            decoder.readPermId();
            decoder.readOutsideRth();
            decoder.readHidden();
            decoder.readDiscretionaryAmount();
            decoder.readGoodAfterTime();
            decoder.readFAParams();
            decoder.readModelCode();
            decoder.readGoodTillDate();
            decoder.readRule80A();
            decoder.readPercentOffset();
            decoder.readSettlingFirm();
            decoder.readShortSaleParams();
            decoder.readBoxOrderParams();
            decoder.readPegToStkOrVolOrderParams();
            decoder.readDisplaySize();
            decoder.readSweepToFill();
            decoder.readAllOrNone();
            decoder.readMinQty();
            decoder.readOcaType();
            decoder.readTriggerMethod();
            decoder.readVolOrderParams(false);
            decoder.readTrailParams();
            decoder.readComboLegs();
            decoder.readSmartComboRoutingParams();
            decoder.readScaleOrderParams();
            decoder.readHedgeParams();
            decoder.readClearingParams();
            decoder.readNotHeld();
            decoder.readDeltaNeutral();
            decoder.readAlgoParams();
            decoder.readSolicited();
            decoder.readOrderStatus();
            decoder.readVolRandomizeFlags();
            decoder.readPegToBenchParams();
            decoder.readConditions();
            decoder.readStopPriceAndLmtPriceOffset();
            decoder.readCashQty();
            decoder.readDontUseAutoPriceForHedge();
            decoder.readIsOmsContainer();
            decoder.readAutoCancelDate();
            decoder.readFilledQuantity();
            decoder.readRefFuturesConId();
            decoder.readAutoCancelParent();
            decoder.readShareholder();
            decoder.readImbalanceOnly();
            decoder.readRouteMarketableToBbo();
            decoder.readParentPermId();
            decoder.readCompletedTime();
            decoder.readCompletedStatus();
        }
    }

    public class CompletedOrdersEnd { }
}