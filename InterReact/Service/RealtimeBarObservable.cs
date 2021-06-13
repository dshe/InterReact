using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which continually emits 5 second bars.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<IRealtimeBar> CreateRealtimeBarObservable(Contract contract,
            RealtimeBarType? whatToShow = null, bool regularTradingHours = true, IList<Tag>? options = null)
        {
            whatToShow ??= RealtimeBarType.Trades;

            return Response.ToObservableWithIdContinuous<IRealtimeBar>(
                    Request.GetNextId,
                    requestId => Request.RequestRealTimeBars(requestId, contract, whatToShow, regularTradingHours, options),
                    requestId => Request.CancelRealTimeBars(requestId))
                .Publish().RefCount();
        }
    }

    public static partial class Extensions
    {
        public static IObservable<RealtimeBar> OfTypeRealtimeBar(this IObservable<IRealtimeBar> source) =>
            source.OfType<RealtimeBar>();
    }

}
