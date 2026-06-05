namespace InterReact;

[Message]
public sealed record TickSnapshotEnd : IHasRequestId
{
    public int RequestId { get; }
    internal TickSnapshotEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
};
