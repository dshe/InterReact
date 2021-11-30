namespace InterReact;

public sealed class RerouteMktData : IHasRequestId
{
    public int RequestId { get; }
    public int ContractId { get; }
    public string Exchange { get; } = "";

    internal RerouteMktData() { }

    internal RerouteMktData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ContractId = r.ReadInt();
        Exchange = r.ReadString();
    }
}

public sealed class RerouteMktDepth : IHasRequestId
{
    public int RequestId { get; }
    public int ContractId { get; }
    public string Exchange { get; } = "";

    internal RerouteMktDepth() { }

    internal RerouteMktDepth(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ContractId = r.ReadInt();
        Exchange = r.ReadString();
    }
}
