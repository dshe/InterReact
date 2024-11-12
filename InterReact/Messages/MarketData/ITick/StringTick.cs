using System.Globalization;
namespace InterReact;

public sealed class StringTick : ITick
{
    public int RequestId { get; private init; }
    public TickType TickType { get; private init; } = TickType.Undefined;
    public string Value { get; private init; } = "";
    private StringTick(int requestId, TickType tickType, string value) 
        => (RequestId, TickType, Value) = (requestId, tickType, value);

    internal static ITick Create(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadEnum<TickType>();
        string str = r.ReadString();
        if (tickType == TickType.LastTimeStamp || tickType == TickType.DelayedLastTimeStamp)
            return new TimeTick(requestId, tickType, str);
        if (tickType == TickType.RealtimeVolume) // there is no delayed RealTimeVolue
            return new RealtimeVolumeTick(requestId, tickType, r.Parser, str);
        return new StringTick(requestId, tickType, str);
    }
}
