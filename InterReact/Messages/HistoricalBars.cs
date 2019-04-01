using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using NodaTime;
using NodaTime.Text;
using System;
using System.Collections.Generic;

namespace InterReact.Messages
{
    public sealed class HistoricalBars : IHasRequestId // output
    {
        internal readonly static LocalDateTimePattern DateTimePattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd HH:mm:ss");

        public int RequestId { get; }
        public LocalDateTime Start { get; }
        public LocalDateTime End { get; }
        public IList<HistoricalBar> Bars { get; } = new List<HistoricalBar>();
        internal HistoricalBars(ResponseComposer c) // a one-shot deal
        {
            if (!c.Config.SupportsServerVersion(ServerVersion.SyntRealtimeBats))
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

        internal HistoricalBar(ResponseComposer c)
        {
            Date = c.ReadLocalDateTime(HistoricalBars.DateTimePattern);
            Open = c.ReadDouble();
            High = c.ReadDouble();
            Low = c.ReadDouble();
            Close = c.ReadDouble();
            Volume = c.Config.ServerVersionCurrent < ServerVersion.SyntRealtimeBats ? c.ReadInt() : c.ReadLong();
            WeightedAveragePrice = c.ReadDouble();
            if (!c.Config.SupportsServerVersion(ServerVersion.SyntRealtimeBats))
                c.ReadString(); /*string hasGaps = */
            Count = c.ReadInt();
        }
    }
}
