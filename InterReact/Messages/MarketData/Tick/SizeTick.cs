namespace InterReact;

/// <summary>
/// Size only. For example, a trade/bid/ask at the previous trade/bid/ask price.
/// </summary>
[Message]
public sealed record SizeTick : TickBase
{
    public decimal Size { get; init; }
    internal SizeTick() { }
    internal SizeTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Size = r.ReadDecimal();
    }
}
