using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public async Task<IList<ITick>> GetTickSnapshotAsync(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
    {
        int requestId = Request.GetNextId();

        Task<IList<IHasRequestId>> task = Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(x => x is SnapshotEndTick || x is Alert)
            .Where(m => m is not SnapshotEndTick)
            .ToList()
            .ToTask();

        Request.RequestMarketData(requestId, contract, genericTickTypes, true, isRegulatorySnapshot, options);

        IList<IHasRequestId> list = await task.ConfigureAwait(false);

        Alert? alert = list.OfType<Alert>().FirstOrDefault();
        if (alert != null)
            throw new Alert().ToException();

        return list.Cast<ITick>().ToList();
    }
}
