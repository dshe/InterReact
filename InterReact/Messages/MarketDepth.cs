using InterReact.Enums;
using InterReact.Interfaces;
using Stringification;

namespace InterReact.Messages
{
    public sealed class MarketDepth : IHasRequestId
    {
        public int RequestId { get; internal set; }

        public int Position { get; internal set; } = -1; // stringify always

        public string MarketMaker { get; internal set; }

        public MarketDepthOperation Operation { get; internal set; } = MarketDepthOperation.Undefined;

        public MarketDepthSide Side { get; internal set; } = MarketDepthSide.Undefined;

        public double Price { get; internal set; }

        public int Size { get; internal set; }
    }

    public sealed class DepthMktDataDescription
    {
        public string Exchange { get; internal set; }
        public string SecType { get; internal set; }
        public string ListingExch { get; internal set; }
        public string ServiceDataTyp { get; internal set; }
        public int? AggGroup { get; internal set; }
    }

}
