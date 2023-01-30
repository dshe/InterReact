namespace InterReact;

public sealed class TickNews : IHasRequestId
{
    public int RequestId { get; }
    public long TimeStamp { get; }
    public string ProviderCode { get; } = "";
    public string ArticleId { get; } = "";
    public string Headline { get; } = "";
    public string ExtraData { get; } = "";

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
