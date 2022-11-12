using System.Collections.Generic;

namespace InterReact;

public sealed class MarketDepthExchanges
{
    public IList<MarketDepthExchange> Exchanges { get; } = new List<MarketDepthExchange>();

    internal MarketDepthExchanges() { }

    internal MarketDepthExchanges(ResponseReader r)
    {
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Exchanges.Add(new MarketDepthExchange(r));
    }
}

public sealed class MarketDepthExchange
{
    public string Exchange { get; } = "";
    public string SecType { get; } = "";
    public string ListingExch { get; } = "";
    public string ServiceDataTyp { get; } = "";
    public int? AggGroup { get; } // The aggregated group

    internal MarketDepthExchange() { }

    internal MarketDepthExchange(ResponseReader r)
    {
        Exchange = r.ReadString();
        SecType = r.ReadString();
        if (r.Connector.SupportsServerVersion(ServerVersion.SERVICE_DATA_TYPE))
        {
            ListingExch = r.ReadString();
            ServiceDataTyp = r.ReadString();
            AggGroup = r.ReadIntNullable();
        }
        else
        {
            ListingExch = "";
            ServiceDataTyp = r.ReadBool() ? "Deep2" : "Deep";
        }
    }
}
