namespace InterReact;

[Message]
public sealed record OrderBound
{
    public long PermId { get; init; }
    public int ApiClientId { get; init; }
    public int ApiOrderId { get; init; }
    internal OrderBound() { }
    internal OrderBound(ResponseReader r)
    {
        PermId = r.ReadLong(); // permId?
        ApiClientId = r.ReadInt(); // ApiClientId
        ApiOrderId = r.ReadInt(); // ApiOrderId
    }
}
