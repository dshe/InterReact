using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns CompletedOrder objects.
    /// Concurrent calls are not supported. 
    /// </summary>
    public async Task<IList<CompletedOrder>> GetCompleteOrdersAsync(bool api, CancellationToken ct = default)
    {
        Task<IList<CompletedOrder>> task = Response
            .Where(o => o is CompletedOrder or CompletedOrdersEnd)
            .TakeWhile(o => o is not CompletedOrdersEnd)
            .OfType<CompletedOrder>()
            .ToList()
            .ToTask(ct);
        
        Request.RequestCompletedOrders(api);

        IList<CompletedOrder> completedOrders = await task.ConfigureAwait(false);

        return completedOrders;
    }
}
