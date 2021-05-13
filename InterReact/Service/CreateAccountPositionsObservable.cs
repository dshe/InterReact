using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using InterReact.Extensions;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits account positions, then completes.
        /// </summary>
        public IObservable<AccountPosition> CreateAccountPositionsObservable() =>
            Response
                .ToObservable<AccountPosition, AccountPositionEnd>(
                    Request.RequestAccountPositions,
                    Request.CancelAccountPositions)
                .ToShareSource();
    }
}
