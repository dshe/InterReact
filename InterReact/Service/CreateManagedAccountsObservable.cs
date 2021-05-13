using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using InterReact.Extensions;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<IList<string>> CreateManagedAccountsObservable() =>
            Response
            .ToObservable<ManagedAccounts>(Request.RequestManagedAccounts)
            .Select(m => m.Accounts)
            .ToShareSource();
    }
}
