using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class FundamentalData : IHasRequestId
    {
        public int RequestId { get; }

        public string Data { get; }

        internal FundamentalData(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Data = c.ReadString();
        }
    }
}
