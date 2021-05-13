namespace InterReact
{
    public sealed class TickNews : IHasRequestId
    {
        public int RequestId { get; }
        public long TimeStamp { get; }
        public string ProviderCode { get; }
        public string ArticleId { get; }
        public string Headline { get; }
        public string ExtraData { get; }
        internal TickNews(ResponseReader c)
        {
            RequestId = c.ReadInt();
            TimeStamp = c.ReadLong();
            ProviderCode = c.ReadString();
            ArticleId = c.ReadString();
            Headline = c.ReadString();
            ExtraData = c.ReadString();
        }
    }
}
