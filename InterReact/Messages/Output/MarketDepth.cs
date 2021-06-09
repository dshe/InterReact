using System.Collections.Generic;

namespace InterReact
{
    public sealed class MarketDepth : IHasRequestId
    {
        public int RequestId { get; }

        public int Position { get; } = -1; // stringify always

        public string MarketMaker { get; }

        public MarketDepthOperation Operation { get; } = MarketDepthOperation.Undefined;

        public MarketDepthSide Side { get; } = MarketDepthSide.Undefined;

        public double Price { get; }

        public int Size { get; }

        public bool IsSmartDepth { get; }

        internal MarketDepth(ResponseReader c, bool isLevel2)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Position = c.ReadInt();
            MarketMaker = isLevel2 ? c.ReadString() : string.Empty;
            Operation = c.ReadEnum<MarketDepthOperation>();
            Side = c.ReadEnum<MarketDepthSide>();
            Price = c.ReadDouble();
            Size = c.ReadInt();
        }
    }

    public sealed class MktDepthExchanges
    {
        public List<MktDepthExchange> Exchanges { get; } = new List<MktDepthExchange>();

        internal MktDepthExchanges(ResponseReader c)
        {
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Exchanges.Add(new MktDepthExchange(c));
        }
    }

    public sealed class MktDepthExchange
    {
        public string Exchange { get; }
        public string SecType { get; }
        public string ListingExch { get; }
        public string ServiceDataTyp { get; }
        public int? AggGroup { get; } // The aggregated group
        internal MktDepthExchange(ResponseReader c)
        {
            Exchange = c.ReadString();
            SecType = c.ReadString();
            if (c.Config.SupportsServerVersion(ServerVersion.ServiceDataType))
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
