using InterReact.Core;
using NodaTime;
using System.Globalization;

namespace InterReact.Messages
{
    public sealed class CurrentTime
    {
        public Instant Time { get; }
        internal CurrentTime(ResponseComposer c)
        {
            c.IgnoreVersion();
            Time = Instant.FromUnixTimeSeconds(long.Parse(c.ReadString(), NumberFormatInfo.InvariantInfo));
        }
    }
}
