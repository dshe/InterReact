using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;

namespace InterReact
{
    public sealed class HistoricalBars : IHasRequestId // output
    {
        internal readonly static LocalDateTimePattern DateTimePattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd  HH:mm:ss");

        public int RequestId { get; }
        public LocalDateTime Start { get; }
        public LocalDateTime End { get; }
        public List<HistoricalBar> Bars { get; } = new List<HistoricalBar>();
        internal HistoricalBars(ResponseReader c) // a one-shot deal
        {
            if (!c.Config.SupportsServerVersion(ServerVersion.SyntRealtimeBars))
                c.RequireVersion(3);
            
            RequestId = c.ReadInt();
            Start = c.ReadLocalDateTime(DateTimePattern);
            End = c.ReadLocalDateTime(DateTimePattern);
            var n = c.ReadInt();
            for (var i = 0; i < n; i++)
                Bars.Add(new HistoricalBar(c));
        }
    }

    public sealed class HistoricalBar
    {
        public LocalDateTime Date { get; }
        public double Open { get; }
        public double High { get; }
        public double Low { get; }
        public double Close { get; }
        public long Volume { get; }
        public double WeightedAveragePrice { get; }
        public int Count { get; }

        internal HistoricalBar(ResponseReader c)
        {
            Date = c.ReadLocalDateTime(HistoricalBars.DateTimePattern);
            Open = c.ReadDouble();
            High = c.ReadDouble();
            Low = c.ReadDouble();
            Close = c.ReadDouble();
            Volume = c.ReadLong();
            WeightedAveragePrice = c.ReadDouble();
            if (!c.Config.SupportsServerVersion(ServerVersion.SyntRealtimeBars))
                c.ReadString(); /*string hasGaps = */
            Count = c.ReadInt();
        }
    }
}
