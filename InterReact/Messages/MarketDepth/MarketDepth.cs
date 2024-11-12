namespace InterReact;

public sealed class MarketDepth : IHasRequestId
{
    public int RequestId { get; }
    public int Position { get; }
    public string MarketMaker { get; } = "";
    public MarketDepthOperation Operation { get; }
    public MarketDepthSide Side { get; }
    public double Price { get; }
    public decimal Size { get; }
    public bool IsSmartDepth { get; }

    internal MarketDepth(ResponseReader r, bool isLevel2)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Position = r.ReadInt();
        if (isLevel2)
            MarketMaker = r.ReadString();
        Operation = r.ReadEnum<MarketDepthOperation>();
        Side = r.ReadEnum<MarketDepthSide>();
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
        if (isLevel2)
            IsSmartDepth = r.ReadBool();
    }
}
