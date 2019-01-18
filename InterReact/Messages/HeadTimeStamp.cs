using System;
using System.Collections.Generic;
using System.Text;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HeadTimestamp : IHasRequestId // output
    {
        public int RequestId { get; internal set; }
        public string HeadTimeStamp { get; internal set; }
    }
}
