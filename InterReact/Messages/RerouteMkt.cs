using InterReact.Core;
using InterReact.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterReact.Messages
{
    public class RerouteMktData : IHasRequestId
    {
        public int RequestId { get; }
        public int ContractId { get; }
        internal RerouteMktData(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            ContractId = c.ReadInt();
        }
    }

    public class RerouteMktDepth : IHasRequestId
    {
        public int RequestId { get; }
        public int ContractId { get; }
        public string Exchange { get; }
        internal RerouteMktDepth(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            ContractId = c.ReadInt();
            Exchange = c.ReadString();
        }
    }
}
