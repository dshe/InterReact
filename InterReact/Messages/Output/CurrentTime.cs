using System.Globalization;

namespace InterReact;

public sealed class CurrentTime
{
    public Instant Time { get; }
    internal CurrentTime() { }
    internal CurrentTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Time = Instant.FromUnixTimeSeconds(long.Parse(r.ReadString(), NumberFormatInfo.InvariantInfo));
    }
}
