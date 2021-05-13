namespace InterReact
{
    public sealed class HistoricalNews : IHasRequestId
    {
        public int RequestId { get; }
        public string Time { get; }
        public string ProviderCode { get; }
        public string ArticleId { get; }
        public string Headline { get; }
        internal HistoricalNews(ResponseReader c)
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
        public int RequestId { get; }
        public bool HasMore { get; }
        internal HistoricalNewsEnd(ResponseReader c)
        {
            RequestId = c.ReadInt();
            HasMore = c.ReadBool();
        }
    }

}
