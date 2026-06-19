namespace InterReact;

[Message]
public sealed record TickNews : IHasRequestId
{
    public int RequestId { get; init; }
    public long TimeStamp { get; init; }
    public string ProviderCode { get; init; } = "";
    public string ArticleId { get; init; } = "";
    public string Headline { get; init; } = "";
    public string ExtraData { get; init; } = "";
    internal TickNews() { }
    internal TickNews(ResponseReader r)
    {
        RequestId = r.ReadInt();
        TimeStamp = r.ReadLong();
        ProviderCode = r.ReadString();
        ArticleId = r.ReadString();
        Headline = r.ReadString();
        ExtraData = r.ReadString();
    }
}
