using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which continually emits 5 second bars.
        /// Use CreateRealtimeBarsObservable().Publish()[.RefCount() | .AutoConnect()] to supoort multiple observers.
        /// </summary>
        public IObservable<Union<RealtimeBar, Alert>> CreateRealtimeBarsObservable(Contract contract,
            RealtimeBarType? whatToShow = null, bool regularTradingHours = true, IList<Tag>? options = null)
        {
            whatToShow ??= RealtimeBarType.Trades;

            return Response.ToObservableContinuousWithId(
                    Request.GetNextId,
                    id => Request.RequestRealTimeBars(id, contract, whatToShow, regularTradingHours, options),
                    id => Request.CancelRealTimeBars(id))
                .Select(x => new Union<RealtimeBar, Alert>(x));
        }
    }
}
