namespace InterReact
{
    public sealed class NewsArticle : IHasRequestId
    {
        public int RequestId { get; }
        public int ArticleType { get; }
        public string ArticleText { get; }
        internal NewsArticle(ResponseReader c)
        {
            RequestId = c.ReadInt();
            ArticleType = c.ReadInt();
            ArticleText = c.ReadString();
        }
    }
}
