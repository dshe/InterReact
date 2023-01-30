using System.Collections.Generic;

namespace InterReact;

public sealed class HistogramData : IHasRequestId
{
    public int RequestId { get; internal init; }
    public IList<HistogramEntry> Items { get; } = new List<HistogramEntry>();
    internal HistogramData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(new HistogramEntry(r));
    }
}

public sealed class HistogramEntry
{
    public double Price { get; }
    public decimal Size { get; }

    internal HistogramEntry(ResponseReader r)
    {
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
    }
}
