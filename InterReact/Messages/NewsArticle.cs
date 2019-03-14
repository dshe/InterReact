using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class NewsArticle : IHasRequestId // output
    {
        public int RequestId { get; }
        public int ArticleType { get;}
        public string ArticleText { get;}
        internal NewsArticle(ResponseReader c)
        {
            RequestId = c.Read<int>();
            ArticleType = c.Read<int>();
            ArticleText = c.ReadString();
        }
    }
}
