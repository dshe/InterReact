namespace InterReact;

[Message]
public sealed record DisplayGroupUpdate : IHasRequestId
{
    public int RequestId { get; init; }
    public string ContractInfo { get; init; } = "";
    internal DisplayGroupUpdate() { }
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
    public int RequestId { get; init; }
    public string Groups { get; init; } = "";
    internal DisplayGroups() { }
    internal DisplayGroups(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Groups = r.ReadString();
    }
}
