using System;
using System.Globalization;
using InterReact.Core;
using InterReact.Interfaces;
using NodaTime;

namespace InterReact.Messages
{
    public sealed class RealtimeBar : IHasRequestId
    {
        public int RequestId { get; }
        public Instant Time { get; }
        public double Open { get; }
        public double High { get; }
        public double Low { get; }
        public double Close { get; }
        public long Volume { get; }
        public double Wap { get; }
        public int Count { get; }
        internal RealtimeBar(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Time = Instant.FromUnixTimeSeconds(long.Parse(c.ReadString(), NumberFormatInfo.InvariantInfo));
            Open = c.ReadDouble();
            High = c.ReadDouble();
            Low = c.ReadDouble();
            Close = c.ReadDouble();
            Volume = c.ReadLong();
            Wap = c.ReadDouble();
            Count = c.ReadInt();
        }
    }
}
