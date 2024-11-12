namespace InterReact;

public sealed class HistoricalNews : IHasRequestId
{
    public int RequestId { get; }
    public string Time { get; }
    public string ProviderCode { get; }
    public string ArticleId { get; }
    public string Headline { get; }
    internal HistoricalNews(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Time = r.ReadString();
        ProviderCode = r.ReadString();
        ArticleId = r.ReadString();
        Headline = r.ReadString();
    }
}

public sealed class HistoricalNewsEnd : IHasRequestId
{
    public int RequestId { get; }
    public bool HasMore { get; }
    internal HistoricalNewsEnd() { }
    internal HistoricalNewsEnd(ResponseReader r)
    {
        RequestId = r.ReadInt();
        HasMore = r.ReadBool();
    }
}
