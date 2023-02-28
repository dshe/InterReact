namespace InterReact;

public sealed class HistoricalBidAskTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalBidAskTick> Ticks { get; } = new List<HistoricalBidAskTick>();
    public bool Done { get; }

    internal HistoricalBidAskTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalBidAskTick(r));
        Done = r.ReadBool();
    }

}

public sealed class HistoricalBidAskTick
{
    public long Time { get; }
    public TickAttribBidAsk TickAttribBidAsk { get; }
    public double PriceBid { get; }
    public double PriceAsk { get; }
    public decimal SizeBid { get; }
    public decimal SizeAsk { get; }

    internal HistoricalBidAskTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribBidAsk = new(r.ReadInt());
        PriceBid = r.ReadDouble();
        PriceAsk = r.ReadDouble();
        SizeBid = r.ReadDecimal();
        SizeAsk = r.ReadDecimal();
    }
}
