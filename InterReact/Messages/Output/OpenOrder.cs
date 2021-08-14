namespace InterReact
{
    public sealed class OpenOrder : IHasOrderId
    {
        public int OrderId { get; }
        public Order Order { get; } = new Order();
        public Contract Contract { get; } = new Contract();
        public OrderState OrderState { get; } = new OrderState();

        internal OpenOrder(ResponseReader c)
        {
            int messageVersion = c.Config.SupportsServerVersion(ServerVersion.ORDER_CONTAINER) ? (int)c.Config.ServerVersionCurrent : c.ReadInt();
            OrderDecoder decoder = new(c, Contract, Order, OrderState, messageVersion);

            decoder.readOrderId();
            OrderId = Order.OrderId;
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
            decoder.readClientId();
            decoder.readPermId();
            decoder.readOutsideRth();
            decoder.readHidden();
            decoder.readDiscretionaryAmount();
            decoder.readGoodAfterTime();
            decoder.skipSharesAllocation();
            decoder.readFAParams();
            decoder.readModelCode();
            decoder.readGoodTillDate();
            decoder.readRule80A();
            decoder.readPercentOffset();
            decoder.readSettlingFirm();
            decoder.readShortSaleParams();
            decoder.readAuctionStrategy();
            decoder.readBoxOrderParams();
            decoder.readPegToStkOrVolOrderParams();
            decoder.readDisplaySize();
            decoder.readOldStyleOutsideRth();
            decoder.readBlockOrder();
            decoder.readSweepToFill();
            decoder.readAllOrNone();
            decoder.readMinQty();
            decoder.readOcaType();
            decoder.skipETradeOnly();
            decoder.skipFirmQuoteOnly();
            decoder.skipNbboPriceCap();
            decoder.readParentId();
            decoder.readTriggerMethod();
            decoder.readVolOrderParams(true);
            decoder.readTrailParams();
            decoder.readBasisPoints();
            decoder.readComboLegs();
            decoder.readSmartComboRoutingParams();
            decoder.readScaleOrderParams();
            decoder.readHedgeParams();
            decoder.readOptOutSmartRouting();
            decoder.readClearingParams();
            decoder.readNotHeld();
            decoder.readDeltaNeutral();
            decoder.readAlgoParams();
            decoder.readSolicited();
            decoder.readWhatIfInfoAndCommission();
            decoder.readVolRandomizeFlags();
            decoder.readPegToBenchParams();
                decoder.readConditions();
            decoder.readAdjustedOrderParams();
            decoder.readSoftDollarTier();
            decoder.readCashQty();
            decoder.readDontUseAutoPriceForHedge();
            decoder.readIsOmsContainer();
            decoder.readDiscretionaryUpToLimitPrice();
            decoder.readUsePriceMgmtAlgo();
            decoder.readDuration();
            decoder.readPostToAts();
        }
    }

    public sealed class OpenOrderEnd
    {
        internal OpenOrderEnd(ResponseReader c) => c.IgnoreVersion();
    }

}