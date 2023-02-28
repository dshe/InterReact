namespace InterReact;

public sealed class AccountSummary : IHasRequestId
{
    public int RequestId { get; }
    public string Account { get; }
    public string Tag { get; }
    public string Currency { get; }
    public string Value { get; }
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

public sealed class AccountSummaryEnd : IHasRequestId
{
    public int RequestId { get; }
    internal AccountSummaryEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
}
