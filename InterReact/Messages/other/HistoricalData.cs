namespace InterReact;

//https://ibkrcampus.com/campus/ibkr-api-page/twsapi-doc/#hist-step-size

public sealed class HistoricalData : IHasRequestId
{
    public int RequestId { get; }
    public string Start { get; }
    public string End { get; }
    public IList<HistoricalDataBar> Bars { get; }
    internal HistoricalData(ResponseReader r) // a one-shot deal
    {
        RequestId = r.ReadInt();
        Start = r.ReadString();
        End = r.ReadString();
        int n = r.ReadInt();
        Bars = new List<HistoricalDataBar>(n);
        for (int i = 0; i < n; i++)
            Bars.Add(HistoricalDataBar.CreateHistoricalBar(r, RequestId));
    }
}

public sealed class HistoricalDataBar : IHasRequestId
{
    public int RequestId { get; init; }
    public required string Time { get; init; }
    public double Open { get; init; }
    public double High { get; init; }
    public double Low { get; init; }
    public double Close { get; init; }
    public decimal Volume { get; init; }
    public decimal WAP { get; init; }
    public int Count { get; init; }

    internal static HistoricalDataBar CreateHistoricalBar(ResponseReader r, int requestId)
    {
        return new HistoricalDataBar()
        {
            RequestId = requestId,
            Time = r.ReadString(),
            Open = r.ReadDouble(),
            High = r.ReadDouble(),
            Low = r.ReadDouble(),
            Close = r.ReadDouble(),
            Volume = r.ReadDecimal(),
            WAP = r.ReadDecimal(),
            Count = r.ReadInt()
        };
    }

    internal static HistoricalDataBar CreateUpdateBar(ResponseReader r)
    {
        return new HistoricalDataBar()
        {
            RequestId = r.ReadInt(),
            Count = r.ReadInt(),
            Time = r.ReadString(),
            Open = r.ReadDouble(),
            Close = r.ReadDouble(),
            High = r.ReadDouble(),
            Low = r.ReadDouble(),
            WAP = r.ReadDecimal(),
            Volume = r.ReadDecimal()
        };
    }
}
