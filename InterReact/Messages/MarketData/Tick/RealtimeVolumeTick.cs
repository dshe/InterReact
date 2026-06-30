using System.Globalization;
namespace InterReact;

/// <summary>
/// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
/// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
/// TickRealtimeVolume ticks are obtained by parsing TickString.
/// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
/// </summary>
[Message]
public sealed record RealtimeVolumeTick : TickBase // from StringTick
{
    public double Price { get; init; }
    public long Size { get; init; }
    public long Volume { get; init; }
    public double Vwap { get; init; }
    public bool SingleTrade { get; init; }
    public Instant Instant { get; init; }
    internal RealtimeVolumeTick() { }
    internal RealtimeVolumeTick(int requestId, TickType tickType, ResponseParser parser, string str)
    {
        RequestId = requestId;
        TickType = tickType;
        string[] parts = str.Split(';');
        Price = parser.ParseDouble(parts[0]);
        Size = parser.ParseLong(parts[1]);
        Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo));
        Volume = parser.ParseLong(parts[3]);
        Vwap = parser.ParseDouble(parts[4]);
        SingleTrade = parser.ParseBool(parts[5]);
    }
};

