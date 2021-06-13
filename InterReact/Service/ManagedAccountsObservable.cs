using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<string> CreateManagedAccountsObservable() =>
            Response
                .ToObservableSingle<ManagedAccounts>(Request.RequestManagedAccounts)
                .Select(m => m.Accounts)
                .ToShareSource();
    }
}
