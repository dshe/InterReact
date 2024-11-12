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
        IList<Tag>? options = null) => Response
            .ToObservableWithId<IHasRequestId>(
                Request.GetNextId,
                id => Request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    false,
                    false,
                    options),
                Request.CancelMarketData,
                null)
            .CacheSource(m => m switch
            {
                ITick tick => tick.TickType.ToString(),
                _ => ""
            }, m => false);

    public IObservable<IHasRequestId> CreateMarketDataSnapshotObservable(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null) => Response

            .ToObservableWithId<IHasRequestId>(
                Request.GetNextId,
                id => Request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    true,
                    isRegulatorySnapshot,
                    options),
                Request.CancelMarketData,
                m => m is SnapshotEndTick);

    public async Task<IHasRequestId[]> GetMarketDataSnapshotAsync(
        Contract contract,
        IList<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        IList<Tag>? options = null,
        TimeSpan? timeout = null) =>
            await CreateMarketDataSnapshotObservable(contract, genericTickTypes, isRegulatorySnapshot, options)
                .ToArray()
                .Timeout(timeout ?? TimeSpan.MaxValue);

}
