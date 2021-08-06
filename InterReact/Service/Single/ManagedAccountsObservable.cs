using System;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        public IObservable<string> ManagedAccountsObservable { get; }

        private IObservable<string> CreateManagedAccountsObservable() =>
            Response
                .ToObservableSingle<ManagedAccounts>(Request.RequestManagedAccounts)
                .Select(m => m.Accounts)
                .ShareSource();
    }
}
