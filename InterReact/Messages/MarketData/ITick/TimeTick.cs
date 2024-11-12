using System.Globalization;

namespace InterReact;

public sealed class TimeTick : ITick // from StringTick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    /// <summary>
    /// Seconds precision.
    /// </summary>
    public Instant Time { get; }
    internal TimeTick(int requestId, TickType tickType, string str) 
    {
        RequestId = requestId;
        TickType = tickType;
        Time = Instant.FromUnixTimeSeconds(long.Parse(str, NumberFormatInfo.InvariantInfo));
    }
}
