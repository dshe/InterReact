using System;
using InterReact.Interfaces;
using NodaTime;

namespace InterReact.Messages
{
    public sealed class RealtimeBar : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public Instant Time { get; internal set; }
        public double Open { get; internal set; }
        public double High { get; internal set; }
        public double Low { get; internal set; }
        public double Close { get; internal set; }
        public long Volume { get; internal set; }
        public double Wap { get; internal set; }
        public int Count { get; internal set; }
    }

}
