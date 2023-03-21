using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public async Task<IList<IHasRequestId>> GetTickSnapshotAsync(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, Tag[]? options = null, CancellationToken ct = default)
    {
        int id = Request.GetNextId();
        options ??= Array.Empty<Tag>();

        Task<IList<IHasRequestId>> task = Response
            .WithRequestId(id)
            .TakeUntil(x => x is SnapshotEndTick or AlertMessage { IsFatal: true })
            .Where(m => m is not SnapshotEndTick)
            .ToList()
            .ToTask(ct);

        Request.RequestMarketData(id, contract, genericTickTypes, true, isRegulatorySnapshot, options);

        IList<IHasRequestId> list = await task.ConfigureAwait(false);

        return list;
    }

    /// <summary>
    /// Creates an observable which emits a snapshot of market data ticks, then completes.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateTickSnapshotObservable(
        Contract contract, IEnumerable<GenericTickType>? genericTickTypes = null, bool isRegulatorySnapshot = false, params Tag[] options)
    {
        return Response
            .ToObservableMultipleWithRequestId<SnapshotEndTick>(
                Request.GetNextId,
                requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, true, isRegulatorySnapshot, options))
            .ShareSource();
    }
}
