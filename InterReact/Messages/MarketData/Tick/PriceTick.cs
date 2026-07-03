namespace InterReact;

// There are many tick types, as identified by the enum TickType. For example: TickType.BidSize.
// Each tick type maps to one of the classes below. For example, TickType.BidSize is represented by objects of class type TickSize.
// https://www.interactivebrokers.com/en/software/api/apiguide/tables/tick_types.htm

/// <summary>
/// A trade/bid/ask at a price which is different from the previous trade/bid/ask price.
/// </summary>
[Message]
public sealed record PriceTick : TickBase
{
    public double Price { get; init; }
    public TickAttrib TickAttrib { get; init; }
    internal PriceTick() => TickAttrib = new TickAttrib(0);
    internal static object CreatePriceTick(ResponseReader r)
    {
        r.RequireMessageVersion(3);

        int requestId = r.ReadInt();
        TickType priceTickType = r.ReadEnum<TickType>();
        double price = r.ReadDouble();
        decimal size = r.ReadDecimal();
        TickAttrib tickAttrib = new(r.ReadInt());

        PriceTick priceTick = new()
        {
            RequestId = requestId,
            TickType = priceTickType,
            Price = price,
            TickAttrib = tickAttrib
        };

        TickType sizeTickType = GetSizeTickType(priceTickType);
        if (sizeTickType == TickType.Undefined)
            return priceTick;

        SizeTick sizeTick = new()
        {
            RequestId = requestId,
            TickType = sizeTickType,
            Size = size
        };

        return new object[] { priceTick, sizeTick };
    }

    private static TickType GetSizeTickType(TickType priceTickType) =>
        priceTickType switch
        {
            TickType.BidPrice => TickType.BidSize,
            TickType.AskPrice => TickType.AskSize,
            TickType.LastPrice => TickType.LastSize,
            TickType.DelayedBidPrice => TickType.DelayedBidSize,
            TickType.DelayedAskPrice => TickType.DelayedAskSize,
            TickType.DelayedLastPrice => TickType.DelayedLastSize,
            _ => TickType.Undefined
        };
}

public sealed record TickAttrib
{
    public bool CanAutoExecute { get; }
    public bool PastLimit { get; }
    public bool PreOpen { get; }
    public bool Unreported { get; }
    public bool BidPastLow { get; }
    public bool AskPastHigh { get; }
    internal TickAttrib(int value)
    {
        BitMask mask = new(value);
        CanAutoExecute = mask[0];
        PastLimit = mask[1];
        PreOpen = mask[2];
    }
}
