using System;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits a snapshot of market ticks, then completes.
        /// </summary>
        public IObservable<ITick> CreateTickSnapshotObservable(Contract contract) =>
            Response
                .ToObservableWithIdMultiple<ITick, TickSnapshotEnd>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes: null, isSnapshot: true))
                .ToShareSource();
    }
}
