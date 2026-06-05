namespace InterReact;

[Message]
public sealed record AccountSummary : IHasRequestId
{
    public int RequestId { get; init; }
    public string Account { get; init; } = "";
    public string Tag { get; init; } = "";
    public string Currency { get; init; } = "";
    public string Value { get; init; } = "";
    internal AccountSummary() { }
    internal AccountSummary(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Account = r.ReadString();
        Tag = r.ReadString();
        Value = r.ReadString();
        Currency = r.ReadString();
    }
}

[Message]
public sealed record AccountSummaryEnd : IHasRequestId
{
    public int RequestId { get; init; }
    internal AccountSummaryEnd() { }
    internal AccountSummaryEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
}
