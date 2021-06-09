using System;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits a snapshot of market ticks, then completes.
        /// </summary>
        public IObservable<Tick> CreateTickSnapshotObservable(Contract contract) =>
            Response
                .ToObservableWithIdMultiple<Tick,TickSnapshotEnd>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes: null, isSnapshot: true))
                .ToShareSource();
    }
}
