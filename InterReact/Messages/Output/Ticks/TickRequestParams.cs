namespace InterReact;

public sealed class TickRequestParams : ITick
{
    public TickType TickType { get; } = TickType.Undefined;
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
