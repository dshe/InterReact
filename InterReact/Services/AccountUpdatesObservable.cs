using Stringification;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which, upon subscription, continually emits account update objects for all accounts:
    /// Objects: AccountValue, PortfolioValue, AccountUpdateTime and AccountUpdateEnd.
    /// AccountUpdateEnd is emitted after the initial values for each account have been emitted.
    /// The latest values are cached for replay to new subscribers.
    /// Multiple subscribers are supported. 
    /// </summary>
    public IObservable<object> AccountUpdatesObservable { get; }

    private IObservable<object> CreateAccountUpdatesObservable()
    {
        return Response
            .Where(m => m is AccountValue or PortfolioValue or AccountUpdateTime or AccountUpdateEnd)
            .ToObservableContinuous(
                () => Request.RequestAccountUpdates(subscribe: true),
                () => Request.RequestAccountUpdates(subscribe: false))
            .CacheSource(GetAccountUpdatesCacheKey);
    }

    private static string GetAccountUpdatesCacheKey(object m)
    {
        return m switch
        {
            AccountValue av => $"{av.AccountName}+{av.Key}:{av.Currency}",
            PortfolioValue pv => $"{pv.AccountName}+{(pv.Contract?.Stringify(includeTypeName: false) ?? "")}",
            AccountUpdateTime => "AccountUpdateTime",
            AccountUpdateEnd _ => "AccountUpdateEnd",
            _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
        };
    }
}
