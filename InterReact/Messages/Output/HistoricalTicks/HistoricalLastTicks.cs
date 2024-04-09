namespace InterReact;

public sealed class HistoricalLastTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalLastTick> Ticks { get; } = new List<HistoricalLastTick>();
    public bool Done { get; }

    internal HistoricalLastTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalLastTick(r));
        Done = r.ReadBool();
    }
}

public sealed class HistoricalLastTick
{
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public long Time { get; }
    public TickAttribLast TickAttribLast { get; }
    public double Price { get; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal Size { get; }
    public string Exchange { get; }
    public string SpecialConditions { get; }

    internal HistoricalLastTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribLast = new(r.ReadInt());
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
        Exchange = r.ReadString();
        SpecialConditions = r.ReadString();
    }
}
