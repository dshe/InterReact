using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class SoftDollarTier
    {
        public string Name { get; }
        public string Value { get; }
        public string DisplayName { get; }
        internal SoftDollarTier(ResponseReader c)
        {
            Name = c.ReadString();
            Value = c.ReadString();
            DisplayName = c.ReadString();
        }
    }

    public sealed class SoftDollarTiers : IHasRequestId
    {
        public int RequestId { get; }
        public IReadOnlyList<SoftDollarTier> Tiers { get; }
        internal SoftDollarTiers(ResponseReader c)
        {
            RequestId = c.Read<int>();
            var n = c.Read<int>();
            Tiers = Enumerable.Repeat(new SoftDollarTier(c), n).ToList();
        }
   }
}
