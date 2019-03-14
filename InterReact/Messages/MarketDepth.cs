using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
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
        internal MarketDepth(ResponseReader c, bool isLevel2)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            Position = c.Read<int>();
            MarketMaker = isLevel2 ? c.ReadString() : string.Empty;
            Operation = c.Read<MarketDepthOperation>();
            Side = c.Read<MarketDepthSide>();
            Price = c.Read<double>();
            Size = c.Read<int>();
        }
    }

    public sealed class DepthMktDataDescription
    {
        public string Exchange { get; }
        public string SecType { get; }
        public string ListingExch { get; }
        public string ServiceDataTyp { get; }
        public int? AggGroup { get; }
        internal DepthMktDataDescription(ResponseReader c)
        {
            Exchange = c.ReadString();
            SecType = c.ReadString();
            if (c.Config.SupportsServerVersion(ServerVersion.ServiceDataType))
            {
                ListingExch = c.ReadString();
                ServiceDataTyp = c.ReadString();
                AggGroup = c.Read<int?>();
            }
            else
            {
                ListingExch = "";
                ServiceDataTyp = c.Read<bool>() ? "Deep2" : "Deep";
            }
        }

        internal static IReadOnlyList<DepthMktDataDescription> GetAll(ResponseReader c)
        {
            var n = c.Read<int>();
            return Enumerable.Repeat(new DepthMktDataDescription(c), n).ToList().AsReadOnly();
        }
    }
}
