namespace InterReact;

public interface ITickByTick : IHasRequestId
{
    TickByTickType TickByTickType { get; }
}
