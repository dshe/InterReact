namespace InterReact;

[Message]
public sealed record NewsArticle : IHasRequestId
{
    public int RequestId { get; init; }
    public int ArticleType { get; init; }
    public string ArticleText { get; init; } = "";
    internal NewsArticle() { }
    internal NewsArticle(ResponseReader r)
    {
        RequestId = r.ReadInt();
        ArticleType = r.ReadInt();
        ArticleText = r.ReadString();
    }
}
