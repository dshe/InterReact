namespace InterReact;

public sealed class OrderBound
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
