namespace InterReact;

[Message]
public sealed record MarketDepth : IHasRequestId
{
    public int RequestId { get; init; }
    public int Position { get; init; }
    public string MarketMaker { get; init; } = "";
    public MarketDepthOperation Operation { get; init; } = MarketDepthOperation.Undefined;
    public MarketDepthSide Side { get; init; } = MarketDepthSide.Undefined;
    public double Price { get; init; }
    public decimal Size { get; init; }
    public bool IsSmartDepth { get; init; }
    internal MarketDepth() { }
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
