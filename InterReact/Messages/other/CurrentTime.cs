namespace InterReact;

[Message]
public sealed record CurrentTime
{
    public long Seconds { get; init; }
    internal CurrentTime() { }
    internal CurrentTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Seconds =r.ReadLong();
    }
}
