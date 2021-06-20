using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits account summary items, and possibly alerts.
        /// All items are sent initially, and then only any changes. 
        /// AccountSummaryEnd is emitted after the initial values for each account have been emitted.
        /// </summary>
        public IObservable<Union<AccountSummary, AccountSummaryEnd, Alert>> AccountSummaryObservable { get; }

        private IObservable<Union<AccountSummary, AccountSummaryEnd, Alert>> CreateAccountSummaryObservable()
        {
            return Response
                .ToObservableWithIdContinuous(
                    Request.GetNextId, id => Request.RequestAccountSummary(id), Request.CancelAccountSummary)
                .Select(x => new Union<AccountSummary, AccountSummaryEnd, Alert>(x))
            .ShareSourceCache(GetCacheKey);

            // local
            static string GetCacheKey(Union<AccountSummary, AccountSummaryEnd, Alert> union)
            {
                return union.Source switch
                {
                    AccountSummary a => $"{a.Account}+{a.Currency}+{a.Tag}",
                    AccountSummaryEnd => "AccountSummaryEnd",
                    Alert alert => $"{alert.Code}+{alert.Message}",
                    _ => ""
                };
            }
        }
    }
}
