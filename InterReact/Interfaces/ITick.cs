using InterReact.Enums;

namespace InterReact.Interfaces
{
    public interface ITick : IHasRequestId
    {
        TickType TickType { get; }
    }
}
