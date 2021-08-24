namespace InterReact
{
    public sealed class FundamentalData : IHasRequestId
    {
        public int RequestId { get; }

        public string Data { get; } = "";
        
        internal FundamentalData() { }

        internal FundamentalData(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            Data = r.ReadString();
        }
    }
}
