namespace InterReact;
//public static readonly ErrorResponse HistoricalDataServiceError = new(162, "Historical market data Service error message.");

[Message]
public sealed record HistoricalTicks : IHasRequestId
{
    public int RequestId { get; init; }
    public IList<HistoricalTick> Ticks { get; init; }
    public bool Done { get; init; }
    internal HistoricalTicks() => Ticks = [];
    internal HistoricalTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Ticks = new List<HistoricalTick>(n);
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalTick(r));
        Done = r.ReadBool();
    }
}

[Message]
public sealed record HistoricalTick
{
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public long Time { get; init; }
    public double Price { get; init; }
    //[return: MarshalAs(UnmanagedType.I8)]
    //[param: MarshalAs(UnmanagedType.I8)]
    public decimal Size { get; init; }
    internal HistoricalTick(ResponseReader r)
    {
        Time = r.ReadLong();
        r.ReadString();
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
    }
}
