using System.Collections.Generic;

namespace InterReact;

public sealed class HistogramItems : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistogramItem> Items { get; } = new List<HistogramItem>();

    internal HistogramItems() { }
    internal HistogramItems(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(new HistogramItem(r));
    }
}

public sealed class HistogramItem
{
    public double Price { get; }
    public long Size { get; }

    internal HistogramItem() { }
    internal HistogramItem(ResponseReader r)
    {
        Price = r.ReadDouble();
        Size = r.ReadLong();
    }
}
