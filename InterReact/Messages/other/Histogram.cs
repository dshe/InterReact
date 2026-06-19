namespace InterReact;

[Message]
public sealed record HistogramData : IHasRequestId
{
    public int RequestId { get; init; }
    public IList<HistogramEntry> Items { get; } = [];
    internal HistogramData() {}
    internal HistogramData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(new HistogramEntry(r));
    }
}

public sealed record HistogramEntry
{
    public double Price { get; init; }
    public decimal Size { get; init; }
    internal HistogramEntry() { }
    internal HistogramEntry(ResponseReader r)
    {
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
    }
}
