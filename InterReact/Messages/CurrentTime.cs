using NodaTime;

namespace InterReact.Messages
{
    public sealed class CurrentTime
    {
        public Instant Time { get; internal set; }
    }
}
