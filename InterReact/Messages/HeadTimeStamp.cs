using System;
using System.Collections.Generic;
using System.Text;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HeadTimestamp : IHasRequestId // output
    {
        public int RequestId { get; }
        public string HeadTimeStamp { get;}

        internal HeadTimestamp(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            HeadTimeStamp = c.ReadString();
        }
    }
}
