using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits one or more account summary items, then completes.
        /// </summary>
        public IObservable<Union<AccountSummary, Alert>> CreateAccountSummaryObservable() =>
            Response
                .ToObservableWithIdMultiple<AccountSummaryEnd>(
                    Request.GetNextId, id => Request.RequestAccountSummary(id), Request.CancelAccountSummary)
                .Select(x => new Union<AccountSummary, Alert>(x))
                .ToShareSource();
    }
}
