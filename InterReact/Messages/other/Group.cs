namespace InterReact;

[Message]
public sealed record DisplayGroupUpdate : IHasRequestId
{
    public int RequestId { get; }
    public string ContractInfo { get; }
    internal DisplayGroupUpdate(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        ContractInfo = r.ReadString();
    }
}

[Message]
public sealed record DisplayGroups : IHasRequestId
{
    public int RequestId { get; }
    public string Groups { get; }

    internal DisplayGroups(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Groups = r.ReadString();
    }
}
