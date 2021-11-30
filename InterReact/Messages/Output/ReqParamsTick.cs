namespace InterReact;

public sealed class ReqParamsTick : Tick
{
    public double MinTick { get; }
    public string BboExchange { get; } = "";
    public int SnapshotPermissions { get; }

    internal ReqParamsTick() { }

    internal ReqParamsTick(ResponseReader r)
    {
        RequestId = r.ReadInt();
        MinTick = r.ReadDouble();
        BboExchange = r.ReadString();
        SnapshotPermissions = r.ReadInt();
    }
};
