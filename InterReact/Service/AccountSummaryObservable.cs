using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits one or more account summary items, then completes.
        /// </summary>
        public IObservable<IAccountSummary> CreateAccountSummaryObservable() =>
            Response
                .ToObservableWithIdMultiple<IAccountSummary, AccountSummaryEnd>(
                    Request.GetNextId,
                    id => Request.RequestAccountSummary(id),
                    Request.CancelAccountSummary)
                .ToShareSource();
    }

    public static partial class Extensions
    {
        public static IObservable<AccountSummary> OfTypeAccountSummary(this IObservable<IAccountSummary> source) => source.OfType<AccountSummary>();
    }

}