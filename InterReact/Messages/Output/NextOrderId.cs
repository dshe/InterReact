using Microsoft.Extensions.Logging;

namespace InterReact;

public sealed class NextOrderId : IHasOrderId
{
    public int OrderId { get; }

    internal NextOrderId() { }

    internal NextOrderId(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        OrderId = r.ReadInt();
 
        int id = r.Connector.Id;
        r.Connector.Id = Math.Max(OrderId - 1, id);
        r.Logger.LogDebug("Received NextOrderId = {OrderId}; Id = {Id1} => {Id2}", OrderId, id, r.Connector.Id);
    }
}
