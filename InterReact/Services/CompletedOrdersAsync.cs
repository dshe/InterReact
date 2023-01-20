using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    public async Task<IList<CompletedOrder>> GetCompleteOrdersAsync(bool api)
    {
        Task<IList<CompletedOrder>> task = Response
            .Where(o => o is CompletedOrder || o is CompletedOrdersEnd)
            .TakeUntil(o => o is CompletedOrdersEnd)
            .OfType<CompletedOrder>()
            .ToList()
            .ToTask();
        
        Request.RequestCompletedOrders(api);

        IList<CompletedOrder> completedOrders = await task.ConfigureAwait(false);

        return completedOrders;
    }
}
