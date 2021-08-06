using System.Collections.Generic;

namespace InterReact
{
    public sealed class MarketDepthExchanges
    {
        public List<MarketDepthExchange> Exchanges { get; } = new();

        internal MarketDepthExchanges(ResponseReader c)
        {
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Exchanges.Add(new MarketDepthExchange(c));
        }
    }

    public sealed class MarketDepthExchange
    {
        public string Exchange { get; }
        public string SecType { get; }
        public string ListingExch { get; }
        public string ServiceDataTyp { get; }
        public int? AggGroup { get; } // The aggregated group
        internal MarketDepthExchange(ResponseReader c)
        {
            Exchange = c.ReadString();
            SecType = c.ReadString();
            if (c.Config.SupportsServerVersion(ServerVersion.SERVICE_DATA_TYPE))
            {
                ListingExch = c.ReadString();
                ServiceDataTyp = c.ReadString();
                AggGroup = c.ReadIntNullable();
            }
            else
            {
                ListingExch = "";
                ServiceDataTyp = c.ReadBool() ? "Deep2" : "Deep";
            }
        }
    }
}
