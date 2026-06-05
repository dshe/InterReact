namespace InterReact;

[Message]
public sealed record CurrentTime
{
    public long Seconds { get; }
    internal CurrentTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Seconds =r.ReadLong();
    }
}
