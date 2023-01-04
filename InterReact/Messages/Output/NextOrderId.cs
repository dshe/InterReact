namespace InterReact;

public sealed class NextOrderId : IHasOrderId
{
    public int OrderId { get; }

    internal NextOrderId() { }

    internal NextOrderId(ResponseReader r)
    {
        r.IgnoreVersion();
        OrderId = r.ReadInt();
    }
}
