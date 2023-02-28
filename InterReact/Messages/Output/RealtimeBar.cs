namespace InterReact;

public sealed class RealtimeBar : IHasRequestId
{
    public int RequestId { get; }
    public Instant Time { get; }
    public double Open { get; }
    public double High { get; }
    public double Low { get; }
    public double Close { get; }
    public decimal Volume { get; }
    public decimal Wap { get; }
    public int Count { get; }

    internal RealtimeBar(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Time = Instant.FromUnixTimeSeconds(r.ReadLong());
        Open = r.ReadDouble();
        High = r.ReadDouble();
        Low = r.ReadDouble();
        Close = r.ReadDouble();
        Volume = r.ReadDecimal();
        Wap = r.ReadDecimal();
        Count = r.ReadInt();
    }
}
