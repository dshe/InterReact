namespace InterReact;

/// <summary>
/// Size only. For example, a trade/bid/ask at the previous trade/bid/ask price.
/// </summary>
public sealed class SizeTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public decimal Size { get; }
    internal SizeTick()
    {
        TickType = TickType.Undefined;
    }
    internal SizeTick(int requestId, TickType tickType, decimal size)
    {
        RequestId = requestId;
        TickType = tickType;
        Size = size;
    }
    internal SizeTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Size = r.ReadDecimal();
    }
}
