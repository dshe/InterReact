using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits one or more account positions, then completes.
        /// </summary>
        public IObservable<AccountPosition> CreateAccountPositionsObservable() =>
            Response
                .Where(x => x is AccountPosition || x is AccountPositionEnd)
                .ToObservableMultiple<AccountPositionEnd>(
                    Request.RequestAccountPositions,
                    Request.CancelAccountPositions)
                .Cast<AccountPosition>()
                .ToShareSource();
    }
}
