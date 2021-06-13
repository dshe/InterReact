using System.Globalization;
using NodaTime;

namespace InterReact
{
    public interface IRealtimeBar : IHasRequestId { }

    public sealed class RealtimeBar : IRealtimeBar
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
