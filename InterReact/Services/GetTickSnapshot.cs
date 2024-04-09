using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public Task<IList<IHasRequestId>> GetTickSnapshotAsync(
        Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        CancellationToken ct = default,
        params Tag[] options)
    {
        int id = Request.GetNextId();

        Request.RequestMarketData(
            id,
            contract,
            genericTickTypes,
            true,
            isRegulatorySnapshot,
            options);

        return Response
            .WithRequestId(id)
            .AlertMessageToError()
            .TakeWhile(m => m is not SnapshotEndTick)
            .ToList()
            .ToTask(ct);
    }
}
