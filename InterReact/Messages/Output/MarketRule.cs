using System.Collections.Generic;

namespace InterReact
{
    public sealed class MarketRule
    {
        public int MarketRuleId { get; }
        public List<PriceIncrement> PriceIncrements { get; } = new List<PriceIncrement>();
        internal MarketRule(ResponseReader c)
        {
            MarketRuleId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                PriceIncrements.Add(new PriceIncrement(c));
        }
    }

    public sealed class PriceIncrement
    {
        public double LowEdge { get; }
        public double Increment { get; }
        public PriceIncrement(ResponseReader c)
        {
            LowEdge = c.ReadDouble();
            Increment = c.ReadDouble();
        }
    }

}
