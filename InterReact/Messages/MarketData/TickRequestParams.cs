namespace InterReact;

[Message]
public sealed record TickRequestParams : IHasRequestId
{
    public int RequestId { get; }
    public double MinTick { get; }
    public string BboExchange { get; }
    public int SnapshotPermissions { get; }
    internal TickRequestParams(ResponseReader r)
    {
        RequestId = r.ReadInt();
        MinTick = r.ReadDouble();
        BboExchange = r.ReadString();
        SnapshotPermissions = r.ReadInt();
    }
};
