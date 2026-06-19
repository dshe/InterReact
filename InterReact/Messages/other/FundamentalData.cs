namespace InterReact;

[Message]
public sealed record FundamentalData : IHasRequestId
{
    public int RequestId { get; init; }
    public string Data { get; init; } = "";
    internal FundamentalData() { }
    internal FundamentalData(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
