using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates a connectable observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, 
        /// release all subscriptions and clear the cache.
        /// </summary>
        public IObservable<Tick> CreateTickObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false,
            IList<Tag>? options = null)
        {
            return Response.
                ToObservableWithIdContinuous<Tick>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId));
        }


        /// <summary>
        /// Creates a connectable observable which continually emits market data ticks for the specified contract.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, 
        /// release all subscriptions and clear the cache.
        /// </summary>
        public IConnectableObservable<Tick> CreateTickConnectableObservable(Contract contract,
            IList<GenericTickType>? genericTickTypes = null, bool marketDataOff = false,
            IList<Tag>? options = null)
        {
            return Response.
                ToObservableWithIdContinuous<Tick>(
                    Request.GetNextId,
                    requestId => Request.RequestMarketData(requestId, contract, genericTickTypes, marketDataOff, false, options),
                    requestId => Request.CancelMarketData(requestId))
                .ToCacheSource(tick => tick.TickType);
        }
    }
}
