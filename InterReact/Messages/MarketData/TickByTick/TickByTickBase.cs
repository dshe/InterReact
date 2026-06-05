namespace InterReact;

public abstract record TickByTickBase : IHasRequestId
{
    public int RequestId { get; protected set; }
    public TickByTickType TickByTickType { get; protected set; }
}
