using System;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits account summary items, then completes.
        /// </summary>
        public IObservable<AccountSummary> CreateAccountSummaryObservable() =>
            Response
                .ToObservableWithIdMultiple<AccountSummary,AccountSummaryEnd>(
                    Request.GetNextId,
                    id => Request.RequestAccountSummary(id),
                    Request.CancelAccountSummary)
                .ToShareSource();
    }
}
