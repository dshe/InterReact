namespace InterReact
{
    public sealed class FundamentalData : IHasRequestId
    {
        public int RequestId { get; }

        public string Data { get; }

        internal FundamentalData(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Data = c.ReadString();
        }
    }
}
