using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class SoftDollarTiers : IHasRequestId
    {
        public int RequestId { get; }
        public IList<SoftDollarTier> Tiers { get; } = new List<SoftDollarTier>();
        internal SoftDollarTiers(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Tiers.Add(new SoftDollarTier(c));
        }
    }

    public sealed class SoftDollarTier
    {
        public string Name { get; }
        public string Value { get; }
        public string DisplayName { get; }
        internal SoftDollarTier(ResponseComposer c)
        {
            Name = c.ReadString();
            Value = c.ReadString();
            DisplayName = c.ReadString();
        }
    }

}
