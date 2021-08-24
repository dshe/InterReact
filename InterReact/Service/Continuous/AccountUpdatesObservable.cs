using System;
using System.Reactive.Linq;
using Stringification;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which continually emits account update objects for all accounts.
        /// AccountUpdateEnd is emitted after the initial values for each account have been emitted.
        /// Use CreateAccountUpdatesObservable().Publish()[.RefCount() | .AutoConnect()] to support multiple observers.
        /// Use CreateAccountUpdatesObservable().CacheSource(Services.GetAccountUpdatesCacheKey)
        /// to cache the latest values for replay to new subscribers.
        /// </summary>
        public IObservable<Union<AccountValue, PortfolioValue, AccountUpdateTime, AccountUpdateEnd>> CreateAccountUpdatesObservable()
        {
            return Response
                .Where(x => x is AccountValue || x is PortfolioValue || x is AccountUpdateTime || x is AccountUpdateEnd)
                .ToObservableContinuous(
                () => Request.RequestAccountUpdates(subscribe: true),
                () => Request.RequestAccountUpdates(subscribe: false))
            .Select(x => new Union<AccountValue, PortfolioValue, AccountUpdateTime, AccountUpdateEnd>(x));
        }

        public static string GetAccountUpdatesCacheKey(Union<AccountValue, PortfolioValue, AccountUpdateTime, AccountUpdateEnd> union)
        {
            return union.Source switch
            {
                AccountValue av => $"{av.Account}+{av.Key}:{av.Currency}",
                PortfolioValue pv => $"{pv.AccountName}+{(pv.Contract == null ? "" : pv.Contract.Stringify(includeTypeName: false))}",
                AccountUpdateTime => "AccountUpdateTime",
                AccountUpdateEnd end => $"AccountSummaryEnd:{end.Account}",
                Alert alert => $"{alert.Code}+{alert.Message}",
                _ => throw new ArgumentException($"Unhandled type: {union.Source.GetType()}.")
            };
        }
    }
}
