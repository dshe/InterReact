namespace InterReact;

public abstract record TickBase : IHasRequestId
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; } = TickType.Undefined;
}
