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
    public async Task<IList<object>> GetTickSnapshotAsync(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, IEnumerable<Tag>? options = null)
    {
        int id = Request.GetNextId();

        Task<IList<object>> task = Response
            .WithRequestId(id)
            .TakeUntil(x => x is SnapshotEndTick || (x is Alert alert && alert.IsFatal))
            .Where(m => m is not SnapshotEndTick)
            .ToList()
            .ToTask();

        Request.RequestMarketData(id, contract, genericTickTypes, true, isRegulatorySnapshot, options);

        IList<object> list = await task.ConfigureAwait(false);

        return list;
    }
}
