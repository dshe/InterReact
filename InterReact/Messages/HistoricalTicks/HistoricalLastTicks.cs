namespace InterReact;

[Message]
public sealed record HistoricalLastTicks : IHasRequestId
{
    public int RequestId { get; init; }
    public IList<HistoricalLastTick> Ticks { get; } = [];
    public bool Done { get; init; }
    internal HistoricalLastTicks() { }
    internal HistoricalLastTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalLastTick(r));
        Done = r.ReadBool();
    }
}

[Message]
public sealed record HistoricalLastTick
{
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public long Time { get; init; }
    public TickAttribLast TickAttribLast { get; init; }
    public double Price { get; init; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal Size { get; init; }
    public string Exchange { get; init; } = "";
    public string SpecialConditions { get; init; } = "";
    internal HistoricalLastTick() => TickAttribLast = new();
    internal HistoricalLastTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribLast = new TickAttribLast(r.ReadInt());
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
        Exchange = r.ReadString();
        SpecialConditions = r.ReadString();
    }
}
