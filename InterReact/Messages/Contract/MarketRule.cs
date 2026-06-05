namespace InterReact;

[Message]
public sealed record MarketRule
{
    public int MarketRuleId { get; init; }
    public IList<PriceIncrement> PriceIncrements { get; init; }
    internal MarketRule() => PriceIncrements = [];
    internal MarketRule(ResponseReader r)
    {
        MarketRuleId = r.ReadInt();
        int n = r.ReadInt();
        PriceIncrements = new List<PriceIncrement>(n);
        for (int i = 0; i < n; i++)
            PriceIncrements.Add(new PriceIncrement(r));
    }
}

[Message]
public sealed record PriceIncrement
{
    public double LowEdge { get; init; }
    public double Increment { get; init; }
    internal PriceIncrement() { }
    internal PriceIncrement(ResponseReader r)
    {
        LowEdge = r.ReadDouble();
        Increment = r.ReadDouble();
    }
}
