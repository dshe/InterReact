namespace InterReact;

public sealed class MarketRule
{
    public int MarketRuleId { get; }
    public IList<PriceIncrement> PriceIncrements { get; }

    internal MarketRule(ResponseReader r)
    {
        MarketRuleId = r.ReadInt();
        int n = r.ReadInt();
        PriceIncrements = new List<PriceIncrement>(n);
        for (int i = 0; i < n; i++)
            PriceIncrements.Add(new PriceIncrement(r));
    }
}

public sealed class PriceIncrement
{
    public double LowEdge { get; }
    public double Increment { get; }

    internal PriceIncrement(ResponseReader r)
    {
        LowEdge = r.ReadDouble();
        Increment = r.ReadDouble();
    }
}
