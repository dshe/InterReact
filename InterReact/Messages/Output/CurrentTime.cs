using System.Globalization;

namespace InterReact;

public sealed class CurrentTime
{
    public Instant Time { get; }
    internal CurrentTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Time = Instant.FromUnixTimeSeconds(long.Parse(r.ReadString(), NumberFormatInfo.InvariantInfo));
    }
}
