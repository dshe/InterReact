namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which continually emits market data ticks for the specified contract.
    /// Use CreateTickObservable().Publish().[RefCount() | AutoConnect()] to share the subscription.
    /// Or use CreateTickObservable(...).CacheSource(GetTickCacheKey)
    /// to cache the latest values for replay to new subscribers.
    /// Tick class may be selected by using the OfTickClass() extension method.
    /// </summary>
    public IObservable<IHasRequestId> CreateTickObservable(
        Contract contract,
        IEnumerable<GenericTickType>? genericTickTypes = null,
        params Tag[] options)
    {
        return Response
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
            .CacheSource(GetTickCacheKey);
    }

    public static string GetTickCacheKey(IHasRequestId m)
    {
        return m switch
        {
            ITick tick => tick.TickType.ToString(),
            AlertMessage alertMessage => $"Alert: {alertMessage.Code}",
            _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
        };
    }
}
