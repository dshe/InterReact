using System.Globalization;
namespace InterReact;

[Message]
public sealed record TimeTick : TickBase // from StringTick
{
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
