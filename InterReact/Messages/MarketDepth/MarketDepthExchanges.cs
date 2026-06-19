namespace InterReact;

[Message]
public sealed record MarketDepthExchanges
{
    public IList<MarketDepthExchange> Exchanges { get; } = [];
    internal MarketDepthExchanges() { }
    internal MarketDepthExchanges(ResponseReader r)
    {
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Exchanges.Add(new MarketDepthExchange(r));
    }
}

[Message]
public sealed record MarketDepthExchange
{
    public string Exchange { get; init; } = "";
    public string SecType { get; init; } = "";
    public string ListingExch { get; init; } = "";
    public string ServiceDataTyp { get; init; } = "";
    public int AggGroup { get; init; } // The aggregated group
    internal MarketDepthExchange() { }
    internal MarketDepthExchange(ResponseReader r)
    {
        Exchange = r.ReadString();
        SecType = r.ReadString();
        ListingExch = r.ReadString();
        ServiceDataTyp = r.ReadString();
        AggGroup = r.ReadIntMax();
    }
}
