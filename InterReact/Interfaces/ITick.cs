namespace InterReact;

public interface ITick : IHasRequestId
{
    TickType TickType { get; }
}
