using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class SoftDollarTier
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public string DisplayName { get; internal set; }
    }

    public sealed class SoftDollarTiers : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public IReadOnlyList<SoftDollarTier> Tiers { get; internal set; }
    }

}
