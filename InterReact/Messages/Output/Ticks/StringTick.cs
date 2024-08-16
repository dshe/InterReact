using System.Globalization;

namespace InterReact;

public sealed class StringTick : ITick
{
    public int RequestId { get; private init; }
    public TickType TickType { get; private init; } = TickType.Undefined;
    public string Value { get; private init; } = "";
    internal static ITick Create(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadEnum<TickType>();
        string str = r.ReadString();
        if (tickType == TickType.LastTimeStamp || tickType == TickType.DelayedLastTimeStamp)
        {
            return new TimeTick()
            {
                RequestId = requestId,
                TickType = tickType,
                Time = Instant.FromUnixTimeSeconds(long.Parse(str, NumberFormatInfo.InvariantInfo))
            };
        }
        if (tickType == TickType.RealtimeVolume) // there is no delayed RealTimeVolue
        {
            ResponseParser parser = r.Parser;
            string[] parts = str.Split(';');
            return new RealtimeVolumeTick
            {
                RequestId = requestId,
                TickType = tickType,
                Price = parser.ParseDouble(parts[0]),
                Size = parser.ParseLong(parts[1]),
                Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo)),
                Volume = parser.ParseLong(parts[3]),
                Vwap = parser.ParseDouble(parts[4]),
                SingleTrade = parser.ParseBool(parts[5])
            };
        }
        return new StringTick
        {
            RequestId = requestId,
            TickType = tickType,
            Value = str
        };
    }
}

public sealed class TimeTick : ITick // from StringTick
{
    public int RequestId { get; internal init; }
    public TickType TickType { get; internal init; } = TickType.Undefined;
    /// <summary>
    /// Seconds precision.
    /// </summary>
    public Instant Time { get; internal init; }
    internal TimeTick() { }
}

/// <summary>
/// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
/// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
/// TickRealtimeVolume ticks are obtained by parsing TickString.
/// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
/// </summary>
public sealed class RealtimeVolumeTick : ITick // from StringTick
{
    public int RequestId { get; internal init; }
    public TickType TickType { get; internal init; } = TickType.Undefined;
    public double Price { get; internal init; }
    public long Size { get; internal init; }
    public long Volume { get; internal init; }
    public double Vwap { get; internal init; }
    public bool SingleTrade { get; internal init; }
    public Instant Instant { get; internal init; }
    internal RealtimeVolumeTick() { }
};

