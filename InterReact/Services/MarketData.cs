using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// The latest tick values are cachedr replay to new subscribers.
    /// Use CreateTickObservable().Publish().[RefCount() | AutoConnect()] to share the subscription.
    /// Tick class may be selected by using the OfTickClass() extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateMarketDataObservable(
        Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null,
        params Tag[] options) => Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestMarketData(
                    id,
                    contract,
                    genericTickTypes,
                    false,
                    false,
                    options),
                Request.CancelMarketData)
            .CacheSource(m => m switch
            {
                ITick tick => tick.TickType.ToString(),
                _ => ""
            });

    /// <summary>
    /// Creates an observable which emits a snapshot of market data ticks.
    /// Tick class may be selected by using the OfTickClass extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateMarketDataSnapshotObservable(
        Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null,
        bool isRegulatorySnapshot = false,
        params Tag[] options) => Response
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
