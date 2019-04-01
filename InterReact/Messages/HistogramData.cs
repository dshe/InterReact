using System;
using System.Collections.Generic;
using System.Linq;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HistogramItems : IHasRequestId // output
    {
        public int RequestId { get; }
        public IList<HistogramItem> Items { get; } = new List<HistogramItem>();
        internal HistogramItems(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Items.Add(new HistogramItem(c));
        }
    }

    public class HistogramItem
    {
        public double Price { get; }
        public long Size { get; }

        internal HistogramItem(ResponseComposer c)
        {
            Price = c.ReadDouble();
            Size = c.ReadLong();
        }
    }

}
