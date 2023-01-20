using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public async Task<IList<IHasRequestId>> GetTickSnapshotAsync(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
    {
        int requestId = Request.GetNextId();

        Task<IList<IHasRequestId>> task = Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(x => x is SnapshotEndTick || (x is Alert alert && alert.IsFatal))
            .Where(m => m is not SnapshotEndTick)
            .ToList()
            .ToTask();

        Request.RequestMarketData(requestId, contract, genericTickTypes, true, isRegulatorySnapshot, options);

        IList<IHasRequestId> list = await task.ConfigureAwait(false);

        return list;
    }
}
