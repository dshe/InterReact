using System.Collections.Generic;

namespace InterReact
{
    public sealed class MarketRule
    {
        public int MarketRuleId { get; }
        public List<PriceIncrement> PriceIncrements { get; } = new();

        internal MarketRule() { }

        internal MarketRule(ResponseReader r)
        {
            MarketRuleId = r.ReadInt();
            int n = r.ReadInt();
            for (int i = 0; i < n; i++)
                PriceIncrements.Add(new PriceIncrement(r));
        }
    }

    public sealed class PriceIncrement
    {
        public double LowEdge { get; }
        public double Increment { get; }

        public PriceIncrement() { }

        public PriceIncrement(ResponseReader r)
        {
            LowEdge = r.ReadDouble();
            Increment = r.ReadDouble();
        }
    }
}
