using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class FundamentalData : IHasRequestId
    {
        public int RequestId { get; internal set; }

        public string XmlData { get; internal set; }
    }
}
