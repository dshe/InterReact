namespace InterReact;

[Message]
public sealed record OrderBound
{
    public long PermId { get; }
    public int ApiClientId { get; }
    public int ApiOrderId { get; }

    internal OrderBound(ResponseReader r)
    {
        PermId = r.ReadLong(); // permId?
        ApiClientId = r.ReadInt(); // ApiClientId
        ApiOrderId = r.ReadInt(); // ApiOrderId
    }
}
