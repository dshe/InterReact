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
}
