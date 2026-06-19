namespace InterReact;

public abstract record TickByTickBase : IHasRequestId
{
    public int RequestId { get; init; }
    public TickByTickType TickByTickType { get; init; }
}
