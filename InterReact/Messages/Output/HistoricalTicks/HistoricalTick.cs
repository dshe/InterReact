namespace InterReact;

public sealed class HistoricalTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
    public bool Done { get; }

    internal HistoricalTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalTick(r));
        Done = r.ReadBool();
    }

}

public sealed class HistoricalTick
{
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public long Time { get; }
    public double Price { get; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal Size { get; }
    internal HistoricalTick(ResponseReader r)
    {
        Time = r.ReadLong();
        r.ReadString();
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
    }
}
