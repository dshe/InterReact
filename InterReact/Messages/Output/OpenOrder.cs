namespace InterReact;

public sealed class OpenOrder : IHasOrderId
{
    public int OrderId { get; }
    public Order Order { get; } = new();
    public Contract Contract { get; } = new();
    public OrderState OrderState { get; } = new();

    internal OpenOrder(ResponseReader reader)
    {
        OrderDecoder eOrderDecoder = new(reader, Contract, Order, OrderState);

        eOrderDecoder.ReadOrderId();
        OrderId = Order.OrderId;
        eOrderDecoder.ReadContract();

        eOrderDecoder.ReadAction();
        eOrderDecoder.ReadTotalQuantity();
        eOrderDecoder.ReadOrderType();
        eOrderDecoder.ReadLmtPrice();
        eOrderDecoder.ReadAuxPrice();
        eOrderDecoder.ReadTif();
        eOrderDecoder.ReadOcaGroup();
        eOrderDecoder.ReadAccount();
        eOrderDecoder.ReadOpenClose();
        eOrderDecoder.ReadOrigin();
        eOrderDecoder.ReadOrderRef();
        eOrderDecoder.ReadClientId();
        eOrderDecoder.ReadPermId();
        eOrderDecoder.ReadOutsideRth();
        eOrderDecoder.ReadHidden();
        eOrderDecoder.ReadDiscretionaryAmount();
        eOrderDecoder.ReadGoodAfterTime();
        eOrderDecoder.SkipSharesAllocation();
        eOrderDecoder.ReadFaParams();
        eOrderDecoder.ReadModelCode();
        eOrderDecoder.ReadGoodTillDate();
        eOrderDecoder.ReadRule80A();
        eOrderDecoder.ReadPercentOffset();
        eOrderDecoder.ReadSettlingFirm();
        eOrderDecoder.ReadShortSaleParams();
        eOrderDecoder.ReadAuctionStrategy();
        eOrderDecoder.ReadBoxOrderParams();
        eOrderDecoder.ReadPegToStkOrVolOrderParams();
        eOrderDecoder.ReadDisplaySize();
        //eOrderDecoder.ReadOldStyleOutsideRth();
        eOrderDecoder.ReadBlockOrder();
        eOrderDecoder.ReadSweepToFill();
        eOrderDecoder.ReadAllOrNone();
        eOrderDecoder.ReadMinQty();
        eOrderDecoder.ReadOcaType();
        eOrderDecoder.SkipETradeOnly();
        eOrderDecoder.SkipFirmQuoteOnly();
        eOrderDecoder.SkipNbboPriceCap();
        eOrderDecoder.ReadParentId();
        eOrderDecoder.ReadTriggerMethod();
        eOrderDecoder.ReadVolOrderParams(true);
        eOrderDecoder.ReadTrailParams();
        eOrderDecoder.ReadBasisPoints();
        eOrderDecoder.ReadComboLegs();
        eOrderDecoder.ReadSmartComboRoutingParams();
        eOrderDecoder.ReadScaleOrderParams();
        eOrderDecoder.ReadHedgeParams();
        eOrderDecoder.ReadOptOutSmartRouting();
        eOrderDecoder.ReadClearingParams();
        eOrderDecoder.ReadNotHeld();
        eOrderDecoder.ReadDeltaNeutral();
        eOrderDecoder.ReadAlgoParams();
        eOrderDecoder.ReadSolicited();
        eOrderDecoder.ReadWhatIfInfoAndCommission();
        eOrderDecoder.ReadVolRandomizeFlags();
        eOrderDecoder.ReadPegToBenchParams();
        eOrderDecoder.ReadConditions();
        eOrderDecoder.ReadAdjustedOrderParams();
        eOrderDecoder.ReadSoftDollarTier();
        eOrderDecoder.ReadCashQty();
        eOrderDecoder.ReadDontUseAutoPriceForHedge();
        eOrderDecoder.ReadIsOmsContainer();
        eOrderDecoder.ReadDiscretionaryUpToLimitPrice();
        eOrderDecoder.ReadUsePriceMgmtAlgo();
        eOrderDecoder.ReadDuration();
        eOrderDecoder.ReadPostToAts();
        eOrderDecoder.ReadAutoCancelParent();
        eOrderDecoder.ReadPegBestPegMidOrderAttributes();

        reader.ReadString(); // ???
    }
}

public sealed class OpenOrderEnd
{
    internal OpenOrderEnd(ResponseReader r) => r.IgnoreMessageVersion();
}
