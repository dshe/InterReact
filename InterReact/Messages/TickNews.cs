using System;
using System.Collections.Generic;
using System.Text;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class TickNews : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public long TimeStamp { get; internal set; }
        public string ProviderCode { get; internal set; }
        public string ArticleId { get; internal set; }
        public string Headline { get; internal set; }
        public string ExtraData { get; internal set; }
    }
}
