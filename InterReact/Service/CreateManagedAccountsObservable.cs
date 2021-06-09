using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;

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
