namespace InterReact;

[Message]
public sealed record StringTick : TickBase
{
    public string Value { get; init; } = "";
    private StringTick() { }
    internal static TickBase Create(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadEnum<TickType>();
        string str = r.ReadString();
        if (tickType == TickType.LastTimeStamp || tickType == TickType.DelayedLastTimeStamp)
            return new TimeTick(requestId, tickType, str);
        if (tickType == TickType.RealtimeVolume) // there is no delayed RealTimeVolue
            return new RealtimeVolumeTick(requestId, tickType, r.Parser, str);
        return new StringTick()
        {
            RequestId = requestId,
            TickType = tickType,
            Value = str
        };
    }
}
