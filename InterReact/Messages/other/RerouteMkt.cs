namespace InterReact;

[Message]
public sealed record RerouteMktData : IHasRequestId
{
    public int RequestId { get; }
    public int ContractId { get; }
    public string Exchange { get; }

    internal RerouteMktData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ContractId = r.ReadInt();
        Exchange = r.ReadString();
    }
}

[Message]
public sealed class RerouteMktDepth : IHasRequestId
{
    public int RequestId { get; }
    public int ContractId { get; }
    public string Exchange { get; }

    internal RerouteMktDepth(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ContractId = r.ReadInt();
        Exchange = r.ReadString();
    }
}
