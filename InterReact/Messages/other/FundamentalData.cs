namespace InterReact;

[Message]
public sealed record FundamentalData : IHasRequestId
{
    public int RequestId { get; }

    public string Data { get; }

    internal FundamentalData(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
