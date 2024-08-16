namespace InterReact;

public sealed class SnapshotEndTick : ITick
{
    public TickType TickType { get; } = TickType.Undefined;
    public int RequestId { get; }
    internal SnapshotEndTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
};
