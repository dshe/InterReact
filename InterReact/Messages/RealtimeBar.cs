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
        internal RealtimeBar(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            Time = Instant.FromUnixTimeSeconds(long.Parse(c.ReadString(), NumberFormatInfo.InvariantInfo));
            Open = c.Read<double>();
            High = c.Read<double>();
            Low = c.Read<double>();
            Close = c.Read<double>();
            Volume = c.Read<long>();
            Wap = c.Read<double>();
            Count = c.Read<int>();
        }
    }
}
