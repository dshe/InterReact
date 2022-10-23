namespace InterReact;

public sealed class OpenOrder : IHasOrderId
{
    public int OrderId { get; }
    public Order Order { get; } = new();
    public Contract Contract { get; } = new();
    public OrderState OrderState { get; } = new();

    internal OpenOrder() { }

    internal OpenOrder(ResponseReader r)
    {
        int messageVersion = r.Connector.SupportsServerVersion(ServerVersion.ORDER_CONTAINER) ? (int)r.Connector.ServerVersionCurrent : r.ReadInt();
        OrderDecoder decoder = new(r, Contract, Order, OrderState, messageVersion);

        decoder.ReadOrderId();
        OrderId = Order.OrderId;
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
        decoder.ReadClientId();
        decoder.ReadPermId();
        decoder.ReadOutsideRth();
        decoder.ReadHidden();
        decoder.ReadDiscretionaryAmount();
        decoder.ReadGoodAfterTime();
        decoder.SkipSharesAllocation();
        decoder.ReadFAParams();
        decoder.ReadModelCode();
        decoder.ReadGoodTillDate();
        decoder.ReadRule80A();
        decoder.ReadPercentOffset();
        decoder.ReadSettlingFirm();
        decoder.ReadShortSaleParams();
        decoder.ReadAuctionStrategy();
        decoder.ReadBoxOrderParams();
        decoder.ReadPegToStkOrVolOrderParams();
        decoder.ReadDisplaySize();
        decoder.ReadOldStyleOutsideRth();
        decoder.ReadBlockOrder();
        decoder.ReadSweepToFill();
        decoder.ReadAllOrNone();
        decoder.ReadMinQty();
        decoder.ReadOcaType();
        decoder.SkipETradeOnly();
        decoder.SkipFirmQuoteOnly();
        decoder.SkipNbboPriceCap();
        decoder.ReadParentId();
        decoder.ReadTriggerMethod();
        decoder.ReadVolOrderParams(true);
        decoder.ReadTrailParams();
        decoder.ReadBasisPoints();
        decoder.ReadComboLegs();
        decoder.ReadSmartComboRoutingParams();
        decoder.ReadScaleOrderParams();
        decoder.ReadHedgeParams();
        decoder.ReadOptOutSmartRouting();
        decoder.ReadClearingParams();
        decoder.ReadNotHeld();
        decoder.ReadDeltaNeutral();
        decoder.ReadAlgoParams();
        decoder.ReadSolicited();
        decoder.ReadWhatIfInfoAndCommission();
        decoder.ReadVolRandomizeFlags();
        decoder.ReadPegToBenchParams();
        decoder.ReadConditions();
        decoder.ReadAdjustedOrderParams();
        decoder.ReadSoftDollarTier();
        decoder.ReadCashQty();
        decoder.ReadDontUseAutoPriceForHedge();
        decoder.ReadIsOmsContainer();
        decoder.ReadDiscretionaryUpToLimitPrice();
        decoder.ReadUsePriceMgmtAlgo();
        decoder.ReadDuration();
        decoder.ReadPostToAts();
    }
}

public sealed class OpenOrderEnd
{
    internal OpenOrderEnd(ResponseReader r) => r.IgnoreVersion();
}
