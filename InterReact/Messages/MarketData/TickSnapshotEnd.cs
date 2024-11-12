namespace InterReact;

public sealed class SnapshotEndTick : IHasRequestId
{
    public int RequestId { get; }
    internal SnapshotEndTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
};
