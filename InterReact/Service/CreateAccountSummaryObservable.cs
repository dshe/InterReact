using System;

using InterReact.Extensions;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits account summary items, then completes.
        /// </summary>
        public IObservable<AccountSummary> CreateAccountSummaryObservable() =>
            Response
                .ToObservableWithId<AccountSummary,AccountSummaryEnd>(
                    Request.GetNextId,
                    requestId => Request.RequestAccountSummary(requestId),
                    Request.CancelAccountSummary)
                .ToShareSource();
    }
}
