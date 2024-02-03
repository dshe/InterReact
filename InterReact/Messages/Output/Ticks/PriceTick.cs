namespace InterReact;

// There are many tick types, as identified by the enum TickType. For example: TickType.BidSize.
// Each tick type maps to one of the classes below. For example, TickType.BidSize is represented by objects of class type TickSize.
// https://www.interactivebrokers.com/en/software/api/apiguide/tables/tick_types.htm

/// <summary>
/// A trade/bid/ask at a price which is different from the previous trade/bid/ask price.
/// </summary>
public sealed class PriceTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public double Price { get; }
    public TickAttrib TickAttrib { get; }
    internal PriceTick()
    {
        TickType = TickType.Undefined;
        TickAttrib = new TickAttrib();
    }
    internal PriceTick(int requestId, TickType tickType, double price, TickAttrib tickAttrib)
    {
        RequestId = requestId;
        TickType = tickType;
        Price = price;
        TickAttrib = tickAttrib;
    }
    internal static object Create(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        int requestId = r.ReadInt();
        TickType priceTickType = r.ReadEnum<TickType>();
        double price = r.ReadDouble();
        decimal size = r.ReadDecimal();
        TickAttrib tickAttrib = new(r.ReadInt());

        PriceTick priceTick = new(requestId, priceTickType, price, tickAttrib);

        var sizeTickType = GetSizeTickType(priceTickType);
        if (sizeTickType == TickType.Undefined)
            return priceTick;

        SizeTick sizeTick = new(requestId, sizeTickType, size);

        return new object[] { priceTick, sizeTick };
    }

    private static TickType GetSizeTickType(TickType priceTickType)
    {
        return priceTickType switch
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
}

public sealed class TickAttrib
{
    public bool CanAutoExecute { get; }
    public bool PastLimit { get; }
    public bool PreOpen { get; }
    public bool Unreported { get; }
    public bool BidPastLow { get; }
    public bool AskPastHigh { get; }
    internal TickAttrib() { }
    internal TickAttrib(int value)
    {
        CanAutoExecute = value == 1;
        BitMask mask = new(value);
        CanAutoExecute = mask[0];
        PastLimit = mask[1];
        PreOpen = mask[2];
    }
}
