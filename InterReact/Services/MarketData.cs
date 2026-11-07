using System.Reactive.Threading.Tasks;
namespace InterReact;

public partial class Service
{
    public async Task<IHasRequestId[]> GetMarketDataSnapshotAsync(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null,
        CancellationToken ct = default)
    {
        int requestId = _request.GetNextId();

        Task<IHasRequestId[]> task = _response
            .WithRequestId(requestId)
            .TakeUntil(m => m is Alert || m is TickSnapshotEnd)
            .Where(m => m is not TickSnapshotEnd)
            .ToArray()
            .ToTask(ct);

        await _request.RequestMarketDataAsync(requestId, contract, genericTickTypes, true, isRegulatorySnapshot, options).ConfigureAwait(false);

        IHasRequestId[] ticks = await task.ConfigureAwait(false);

        return ticks;
    }

    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// Output messages types are ITick or AlertMessage.
    /// The latest tick values are cached replay to new subscribers.
    /// Tick class may be selected by using the "OfTickClass()" extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateMarketDataObservable(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        IList<Tag>? options = null)
    {
        return _response
            .ToObservableWithId(
                _request.GetNextId,
                id => _request.RequestMarketDataAsync(
                    id,
                    contract,
                    genericTickTypes,
                    false,
                    false,
                    options),
                _request.CancelMarketDataAsync)
            .CacheSource(m => m switch
            {
                TickBase tick => tick.TickType.ToString(),
                _ => ""
            });
    }
}
