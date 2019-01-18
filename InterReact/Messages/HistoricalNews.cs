using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HistoricalNews : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Time { get; internal set; }
        public string ProviderCode { get; internal set; }
        public string ArticleId { get; internal set; }
        public string Headline { get; internal set; }
    }

    public sealed class HistoricalNewsEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public bool HasMore { get; internal set; }
    }

}
