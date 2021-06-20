using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public interface IHistogramItems : IHasRequestId { }

    public sealed class HistogramItems : IHistogramItems
    {
        public int RequestId { get; }
        public List<HistogramItem> Items { get; } = new List<HistogramItem>();
        internal HistogramItems(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
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

    public static partial class Extensions
    {
        public static IObservable<HistogramItems> OfTypeHistogramItems(
            this IObservable<IHistogramItems> source) => source.OfType<HistogramItems>();
    }

}
