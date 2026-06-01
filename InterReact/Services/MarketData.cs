namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// Output messages types are ITick or AlertMessage.
    /// The latest tick values are cached replay to new subscribers.
    /// Tick class may be selected by using the "OfTickClass()" extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateMarketDataObservable(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        IList<Tag>? options = null) => _response
            .ToObservableWithId(
                _request.GetNextId,
                id => _request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    false,
                    false,
                    options),
                _request.CancelMarketData)
            .CacheSource(m => m switch
            {
                TickBase tick => tick.TickType.ToString(),
                _ => ""
            });


    public IObservable<IHasRequestId> CreateMarketDataSnapshotObservable(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null) => _response
            .ToObservableWithId(
                _request.GetNextId,
                id => _request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    true,
                    isRegulatorySnapshot,
                    options),
                _request.CancelMarketData)
            .TakeUntil(m => m is TickSnapshotEnd);


    public async Task<IHasRequestId[]> GetMarketDataSnapshotAsync(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null,
        TimeSpan? timeout = null,
        CancellationToken ct = default) =>
            await CreateMarketDataSnapshotObservable(contract, genericTickTypes, isRegulatorySnapshot, options)
                .WithTimeout(timeout, ct)
                .ToArray();
}
