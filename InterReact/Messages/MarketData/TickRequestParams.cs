namespace InterReact;

[Message]
public sealed record TickRequestParams : IHasRequestId
{
    public int RequestId { get; init; }
    public double MinTick { get; init; }
    public string BboExchange { get; init; } = "";
    public int SnapshotPermissions { get; init; }
    internal TickRequestParams() { }
    internal TickRequestParams(ResponseReader r)
    {
        RequestId = r.ReadInt();
        MinTick = r.ReadDouble();
        BboExchange = r.ReadString();
        SnapshotPermissions = r.ReadInt();
    }
};
