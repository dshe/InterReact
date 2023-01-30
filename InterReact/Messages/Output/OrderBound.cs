namespace InterReact;

public sealed class OrderBound : IHasOrderId
{
    public long OrderBoundId { get; }
    public int ClientId { get; }
    public int OrderId { get; }

    internal OrderBound(ResponseReader r)
    {
        OrderBoundId = r.ReadLong(); // was OrderId, but is a long here! ???
        ClientId = r.ReadInt(); // was ApiClientId
        OrderId = r.ReadInt(); // was ApiIOrderId
    }
}
