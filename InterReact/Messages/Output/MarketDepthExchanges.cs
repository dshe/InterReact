namespace InterReact;

public sealed class MarketDepthExchanges
{
    public IList<MarketDepthExchange> Exchanges { get; }

    internal MarketDepthExchanges(ResponseReader r)
    {
        int n = r.ReadInt();
        Exchanges = new List<MarketDepthExchange>(n);
        for (int i = 0; i < n; i++)
            Exchanges.Add(new MarketDepthExchange(r));
    }
}

public sealed class MarketDepthExchange
{
    public string Exchange { get; }
    public string SecType { get; }
    public string ListingExch { get; }
    public string ServiceDataTyp { get; }
    public int AggGroup { get; } // The aggregated group

    internal MarketDepthExchange(ResponseReader r)
    {
        Exchange = r.ReadString();
        SecType = r.ReadString();
        ListingExch = r.ReadString();
        ServiceDataTyp = r.ReadString();
        AggGroup = r.ReadIntMax();
    }
}
