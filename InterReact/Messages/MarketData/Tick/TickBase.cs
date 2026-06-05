namespace InterReact;

public abstract record TickBase : IHasRequestId
{
    public int RequestId { get; protected set; }
    public TickType TickType { get; protected set; } = TickType.Undefined;
}
