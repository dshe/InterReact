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
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string DisplayName { get; set; } = "";
        internal SoftDollarTier() { }
        internal SoftDollarTier(ResponseReader c) 
        {
            Set(c);
        }
        internal void Set(ResponseReader c)
        {
            Name = c.ReadString();
            Value = c.ReadString();
            DisplayName = c.ReadString();
        }
    }
}
