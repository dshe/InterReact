namespace InterReact;

[Message]
public sealed record TickSnapshotEnd : IHasRequestId
{
    public int RequestId { get; init; }
    internal TickSnapshotEnd() { }
    internal TickSnapshotEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
};
