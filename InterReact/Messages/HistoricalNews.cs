using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HistoricalNews : IHasRequestId
    {
        public int RequestId { get; }
        public string Time { get;}
        public string ProviderCode { get;}
        public string ArticleId { get;}
        public string Headline { get;}
        internal HistoricalNews(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            Time = c.ReadString();
            ProviderCode = c.ReadString();
            ArticleId = c.ReadString();
            Headline = c.ReadString();
        }
    }

    public sealed class HistoricalNewsEnd : IHasRequestId
    {
        public int RequestId { get;}
        public bool HasMore { get;}
        internal HistoricalNewsEnd(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            HasMore = c.ReadBool();
        }
    }

}
