using Stringification;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits AccountValue and PortfolioValue objects for the specified account.
    /// In case there if more than one managed account, the account must be specified.
    /// All data is sent initially, and then only updates.
    /// The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<object> CreateAccountUpdatesObservable(string accountCode = "")
    {
        return Response
            .Where(m => m is AccountValue or PortfolioValue)
            .ToObservableContinuous(
                () => Request.RequestAccountUpdates(subscribe: true, accountCode),
                () => Request.RequestAccountUpdates(subscribe: false, accountCode))
            .CacheSource(m => m switch
            {
                AccountValue av => $"{av.AccountName} AccountValue:   {av.Key}: {av.Currency}",
                PortfolioValue pv => $"{pv.AccountName} PortfolioValue: {(pv.Contract?.Stringify(includeTypeName: false) ?? "")}",
                _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
            });
    }
}
