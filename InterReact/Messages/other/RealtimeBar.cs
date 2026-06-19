namespace InterReact;

[Message]
public sealed record RealtimeBar : IHasRequestId
{
    public int RequestId { get; init; }
    public long Time { get; init; }
    public double Open { get; init; }
    public double High { get; init; }
    public double Low { get; init; }
    public double Close { get; init; }
    public decimal Volume { get; init; }
    public decimal Wap { get; init; }
    public int Count { get; init; }
    internal RealtimeBar() { }
    internal RealtimeBar(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Time = r.ReadLong(); // Instant.FromUnixTimeSeconds(r.ReadLong());
        Open = r.ReadDouble();
        High = r.ReadDouble();
        Low = r.ReadDouble();
        Close = r.ReadDouble();
        Volume = r.ReadDecimal();
        Wap = r.ReadDecimal();
        Count = r.ReadInt();
    }
}
