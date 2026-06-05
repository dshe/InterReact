namespace InterReact;

[Message]
public sealed record HistogramData : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistogramEntry> Items { get; }
    internal HistogramData() => Items = [];
    internal HistogramData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Items = new List<HistogramEntry>(n);
        for (int i = 0; i < n; i++)
            Items.Add(new HistogramEntry(r));
    }
}

[Message]
public sealed record HistogramEntry
{
    public double Price { get; }
    public decimal Size { get; }

    internal HistogramEntry(ResponseReader r)
    {
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
    }
}
