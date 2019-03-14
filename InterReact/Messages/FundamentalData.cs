using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class FundamentalData : IHasRequestId
    {
        public int RequestId { get; }

        public string XmlData { get; }

        internal FundamentalData(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            XmlData = c.ReadString();
        }
    }
}
