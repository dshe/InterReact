using System.Collections.Generic;
using System.Text;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    // output
    public sealed class SmartComponents : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public IReadOnlyDictionary<int, KeyValuePair<string, char>> Map { get; internal set; }
    }
}
