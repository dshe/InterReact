namespace InterReact;

[Message]
public sealed record RerouteMktData : IHasRequestId
{
    public int RequestId { get; init; }
    public int ContractId { get; init; }
    public string Exchange { get; } = "";
    internal RerouteMktData() { }
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
    public int RequestId { get; init; }
    public int ContractId { get; init; }
    public string Exchange { get; init; } = "";
    internal RerouteMktDepth() { }
    internal RerouteMktDepth(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ContractId = r.ReadInt();
        Exchange = r.ReadString();
    }
}
