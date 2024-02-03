namespace InterReact;

public sealed class HistoricalData : IHasRequestId
{
    public int RequestId { get; }
    public string Start { get; }
    public string End { get; }
    public IList<HistoricalDataBar> Bars { get; } = new List<HistoricalDataBar>();

    internal HistoricalData(ResponseReader r) // a one-shot deal
    {
        RequestId = r.ReadInt();
        Start = r.ReadString();
        End = r.ReadString();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Bars.Add(HistoricalDataBar.CreateHistoricalBar(r, RequestId));
    }
}

public sealed class HistoricalDataBar : IHasRequestId
{
    public int RequestId { get; init; }
    public string Date { get; init; } = "";
    public double Open { get; init; }
    public double High { get; init; }
    public double Low { get; init; }
    public double Close { get; init; }
    public decimal Volume { get; init; }
    public decimal WeightedAveragePrice { get; init; }
    public int Count { get; init; }
    private HistoricalDataBar() { }

    internal static HistoricalDataBar CreateHistoricalBar(ResponseReader r, int requestId)
    {
        return new HistoricalDataBar()
        {
            RequestId = requestId,
            Date = r.ReadString(),
            Open = r.ReadDouble(),
            High = r.ReadDouble(),
            Low = r.ReadDouble(),
            Close = r.ReadDouble(),
            Volume = r.ReadDecimal(),
            WeightedAveragePrice = r.ReadDecimal(),
            Count = r.ReadInt()
        };
    }

    internal static HistoricalDataBar CreateUpdateBar(ResponseReader r)
    {
        return new HistoricalDataBar()
        {
            RequestId = r.ReadInt(),
            Count = r.ReadInt(),
            Date = r.ReadString(),
            Open = r.ReadDouble(),
            Close = r.ReadDouble(),
            High = r.ReadDouble(),
            Low = r.ReadDouble(),
            WeightedAveragePrice = r.ReadDecimal(),
            Volume = r.ReadDecimal()
        };
    }
}
