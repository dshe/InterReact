namespace InterReact
{
    public interface IFundamentalData : IHasRequestId { }

    public sealed class FundamentalData : IFundamentalData
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
