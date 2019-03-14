using System;
using System.Collections.Generic;
using System.Linq;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class HistogramData : IHasRequestId // output
    {
        public int RequestId { get; }
        public IReadOnlyList<(double, long)> Data { get; }
        internal HistogramData(ResponseReader c)
        {
            RequestId = c.Read<int>();
            var n = c.Read<int>();
            Data = Enumerable.Repeat((c.Read<double>(), c.Read<long>()), n).ToList();
        }
    }
}
