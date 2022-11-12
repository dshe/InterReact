using System.Reactive.Linq;
using Stringification;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which, upon subscription, continually emits account update objects for all accounts:
    /// AccountValue, PortfolioValue, AccountUpdateTime and AccountUpdateEnd.
    /// AccountUpdateEnd is emitted after the initial values for each account have been emitted.
    /// Multiple subscribers are supported. The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<object> AccountUpdatesObservable { get; }

    private IObservable<object> CreateAccountUpdatesObservable()
    {
        return Response
            .Where(x => x is AccountValue || x is PortfolioValue || x is AccountUpdateTime || x is AccountUpdateEnd)
            .ToObservableContinuous(
                () => Request.RequestAccountUpdates(subscribe: true),
                () => Request.RequestAccountUpdates(subscribe: false))
            .CacheSource(GetAccountUpdatesCacheKey);
    }

    private static string GetAccountUpdatesCacheKey(object o)
    {
        return o switch
        {
            AccountValue av => $"{av.AccountName}+{av.Key}:{av.Currency}",
            PortfolioValue pv => $"{pv.AccountName}+{(pv.Contract?.Stringify(includeTypeName: false) ?? "")}",
            AccountUpdateTime => "AccountUpdateTime",
            AccountUpdateEnd _ => "AccountUpdateEnd",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}
