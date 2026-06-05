namespace InterReact;

[Message]
public sealed record HistoricalBidAskTicks : IHasRequestId
{
    public int RequestId { get; init; }
    public IList<HistoricalBidAskTick> Ticks { get; init; }
    public bool Done { get; init; }
    internal HistoricalBidAskTicks() => Ticks = [];
    internal HistoricalBidAskTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Ticks = new List<HistoricalBidAskTick>(n);
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalBidAskTick(r));
        Done = r.ReadBool();
    }

}

[Message]
public sealed record HistoricalBidAskTick
{
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public long Time { get; init; }
    public TickAttribBidAsk TickAttribBidAsk { get; init; }
    public double PriceBid { get; init; }
    public double PriceAsk { get; init; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal SizeBid { get; init; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal SizeAsk { get; init; }
    internal HistoricalBidAskTick() => TickAttribBidAsk = new();
    internal HistoricalBidAskTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribBidAsk = new TickAttribBidAsk(r.ReadInt());
        PriceBid = r.ReadDouble();
        PriceAsk = r.ReadDouble();
        SizeBid = r.ReadDecimal();
        SizeAsk = r.ReadDecimal();
    }
}
