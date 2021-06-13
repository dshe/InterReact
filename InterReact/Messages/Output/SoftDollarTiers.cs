using System.Collections.Generic;

namespace InterReact
{
    public sealed class SoftDollarTiers : IHasRequestId
    {
        public int RequestId { get; }
        public List<SoftDollarTier> Tiers { get; } = new List<SoftDollarTier>();
        internal SoftDollarTiers(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
            {
                Tiers.Add(new SoftDollarTier(c));
            }
        }
    }

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

}
