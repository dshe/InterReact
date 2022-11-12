using System.Globalization;
using NodaTime;

namespace InterReact;

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

    internal RealtimeBar() { }

    internal RealtimeBar(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
        Time = Instant.FromUnixTimeSeconds(long.Parse(r.ReadString(), NumberFormatInfo.InvariantInfo));
        Open = r.ReadDouble();
        High = r.ReadDouble();
        Low = r.ReadDouble();
        Close = r.ReadDouble();
        Volume = r.ReadLong();
        Wap = r.ReadDouble();
        Count = r.ReadInt();
    }
}
