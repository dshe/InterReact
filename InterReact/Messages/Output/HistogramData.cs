using System.Collections.Generic;

namespace InterReact
{
    public sealed class HistogramItems : IHasRequestId // output
    {
        public int RequestId { get; }
        public List<HistogramItem> Items { get; } = new List<HistogramItem>();
        internal HistogramItems(ResponseReader c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Items.Add(new HistogramItem(c));
        }
    }

    public sealed class HistogramItem
    {
        public double Price { get; }
        public long Size { get; }

        internal HistogramItem(ResponseReader c)
        {
            Price = c.ReadDouble();
            Size = c.ReadLong();
        }
    }

}
