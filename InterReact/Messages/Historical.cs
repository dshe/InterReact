using InterReact.Interfaces;
using NodaTime;
using System;
using System.Collections.Generic;

namespace InterReact.Messages
{
    public sealed class HistoricalBars : IHasRequestId // output
    {
        public int RequestId { get; internal set; }
        public LocalDateTime Start { get; internal set; }
        public LocalDateTime End { get; internal set; }
        public IReadOnlyList<HistoricalBar> Bars { get; internal set; }
    }

    public sealed class HistoricalBar
    {
        public LocalDateTime Date { get; internal set; }
        public double Open { get; internal set; }
        public double High { get; internal set; }
        public double Low { get; internal set; }
        public double Close { get; internal set; }
        public int Volume { get; internal set; }
        public double WeightedAveragePrice { get; internal set; }
        public int Count { get; internal set; }
    }

}
