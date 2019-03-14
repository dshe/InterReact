using InterReact.Core;
using InterReact.Interfaces;
using NodaTime;
using System;
using System.Collections.Generic;

namespace InterReact.Messages
{
    public sealed class HistoricalBars : IHasRequestId // output
    {
        public int RequestId { get; }
        public LocalDateTime Start { get; }
        public LocalDateTime End { get; }
        public IReadOnlyList<HistoricalBar> Bars { get; }
        internal HistoricalBars(ResponseReader c) // a one-shot deal
        {
            c.RequireVersion(3);
            RequestId = c.Read<int>();
            Start = c.Read<LocalDateTime>();
            End = c.Read<LocalDateTime>();
            var n = c.Read<int>();
            var list = new List<HistoricalBar>(n);
            for (var i = 0; i < n; i++)
                list.Add(new HistoricalBar(c));
            Bars = list.AsReadOnly();
        }
    }

    public sealed class HistoricalBar
    {
        public LocalDateTime Date { get; }
        public double Open { get; }
        public double High { get; }
        public double Low { get; }
        public double Close { get; }
        public int Volume { get; }
        public double WeightedAveragePrice { get; }
        public int Count { get; }

        internal HistoricalBar(ResponseReader c)
        {
            Date = c.Read<LocalDateTime>();
            Open = c.Read<double>();
            High = c.Read<double>();
            Low = c.Read<double>();
            Close = c.Read<double>();
            Volume = c.Read<int>();
            WeightedAveragePrice = c.Read<double>();
            Count = c.Read<int>();
        }
    }
}
