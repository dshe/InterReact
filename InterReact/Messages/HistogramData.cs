using System;
using System.Collections.Generic;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HistogramData : IHasRequestId // output
    {
        public int RequestId { get; internal set; }
        public IReadOnlyList<(double, long)> Data { get; internal set; }
    }
}
