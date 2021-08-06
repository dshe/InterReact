using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;

namespace InterReact
{
    public sealed class HistoricalData : IHasRequestId
    {
        internal static readonly LocalDateTimePattern DateTimePattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd  HH:mm:ss");

        public int RequestId { get; }
        public LocalDateTime Start { get; }
        public LocalDateTime End { get; }
        public List<HistoricalDataBar> Bars { get; } = new List<HistoricalDataBar>();
        internal HistoricalData(ResponseReader c) // a one-shot deal
        {
            if (!c.Config.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
                c.RequireVersion(3);
            
            RequestId = c.ReadInt();
            Start = c.ReadLocalDateTime(DateTimePattern);
            End = c.ReadLocalDateTime(DateTimePattern);
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Bars.Add(new HistoricalDataBar(c));
        }
    }

    public sealed class HistoricalDataBar
    {
        public LocalDateTime Date { get; }
        public double Open { get; }
        public double High { get; }
        public double Low { get; }
        public double Close { get; }
        public long Volume { get; }
        public double WeightedAveragePrice { get; }
        public int Count { get; }

        internal HistoricalDataBar(ResponseReader c)
        {
            Date = c.ReadLocalDateTime(HistoricalData.DateTimePattern);
            Open = c.ReadDouble();
            High = c.ReadDouble();
            Low = c.ReadDouble();
            Close = c.ReadDouble();
            Volume = c.ReadLong();
            WeightedAveragePrice = c.ReadDouble();
            if (!c.Config.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
                c.ReadString(); /*string hasGaps = */
            Count = c.ReadInt();
        }
    }
}
