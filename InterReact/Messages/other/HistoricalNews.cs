namespace InterReact;

[Message]
public sealed record HistoricalNews : IHasRequestId
{
    public int RequestId { get; init; }
    public string Time { get; init; } = "";
    public string ProviderCode { get; init; } = "";
    public string ArticleId { get; init; } = "";
    public string Headline { get; init; } = "";
    internal HistoricalNews() { }
    internal HistoricalNews(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Time = r.ReadString();
        ProviderCode = r.ReadString();
        ArticleId = r.ReadString();
        Headline = r.ReadString();
    }
}

[Message]
public sealed record HistoricalNewsEnd : IHasRequestId
{
    public int RequestId { get; init; }
    public bool HasMore { get; init; }
    internal HistoricalNewsEnd() { }
    internal HistoricalNewsEnd(ResponseReader r)
    {
        RequestId = r.ReadInt();
        HasMore = r.ReadBool();
    }
}
