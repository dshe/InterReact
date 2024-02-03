namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateTickSnapshotObservable(
        Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        params Tag[] options)
    {
        return Response
            .ToObservableMultipleWithId<SnapshotEndTick>(
                Request.GetNextId,
                id => Request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    true,
                    isRegulatorySnapshot,
                    options))
            .ShareSource();
    }
}
