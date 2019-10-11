using InterReact.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class MarketRule
    {
        public int MarketRuleId { get; }
        public IList<PriceIncrement> PriceIncrements { get; } = new List<PriceIncrement>();
        internal MarketRule(ResponseComposer c)
        {
            MarketRuleId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                PriceIncrements.Add(new PriceIncrement(c));
        }
    }

    public sealed class PriceIncrement
    {
        public double LowEdge { get; }
        public double Increment { get; }
        public PriceIncrement(ResponseComposer c)
        {
            LowEdge = c.ReadDouble();
            Increment = c.ReadDouble();
        }
    }

}
