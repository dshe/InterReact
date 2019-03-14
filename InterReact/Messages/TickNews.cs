using System;
using System.Collections.Generic;
using System.Text;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class TickNews : IHasRequestId
    {
        public int RequestId { get; }
        public long TimeStamp { get; }
        public string ProviderCode { get; }
        public string ArticleId { get; }
        public string Headline { get; }
        public string ExtraData { get;}
        internal TickNews(ResponseReader c)
        {
            RequestId = c.Read<int>();
            TimeStamp = c.Read<long>();
            ProviderCode = c.ReadString();
            ArticleId = c.ReadString();
            Headline = c.ReadString();
            ExtraData = c.ReadString();
        }
    }
}
