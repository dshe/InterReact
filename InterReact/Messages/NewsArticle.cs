using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class NewsArticle : IHasRequestId // output
    {
        public int RequestId { get; internal set; }
        public int ArticleType { get; internal set; }
        public string ArticleText { get; internal set; }
    }
}
