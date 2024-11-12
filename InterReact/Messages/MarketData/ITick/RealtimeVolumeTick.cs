using System.Globalization;
namespace InterReact;

/// <summary>
/// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
/// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
/// TickRealtimeVolume ticks are obtained by parsing TickString.
/// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
/// </summary>
public sealed class RealtimeVolumeTick : ITick // from StringTick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public double Price { get; }
    public long Size { get; }
    public long Volume { get; }
    public double Vwap { get; }
    public bool SingleTrade { get; }
    public Instant Instant { get; }
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

