using Stringification;
using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits AccountValue, PortfolioValue and AccountUpdateTime messages.
    /// In case there if more than one managed account, the account must be specified.
    /// All data is sent initially, and then only updates.
    /// AccountUpdateEnd indicates all the messages have been transmitted.
    /// The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<object> CreateAccountUpdatesObservable(string accountCode = "")
    {
        return Response
            .Where(m => m is AccountValue or PortfolioValue or AccountUpdateTime or AccountUpdateEnd)
            .ToObservableContinuous(
                () => Request.RequestAccountUpdates(subscribe: true, accountCode),
                () => Request.RequestAccountUpdates(subscribe: false, accountCode))
            .CacheSource(m => m switch
            {
                AccountValue a => $"{a.AccountName} AccountValue: {a.Key}: {a.Currency}",
                PortfolioValue p => $"{p.AccountName} PortfolioValue: {(p.Contract?.Stringify(includeTypeName: false) ?? "")}",
                _ => ""
            });
    }

    public async Task<IList<object>> GetAccountUpdatesAsync(string accountCode = "", CancellationToken ct = default) =>
        await CreateAccountUpdatesObservable(accountCode)
            .TakeWhile(m => m is not AccountUpdateEnd)
            .ToList()
            .ToTask(ct)
            .ConfigureAwait(false);

    /// <summary>
    /// Creates an observable which emits AccountUpdateMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// AccountSummaryEnd indicates all the messages have been transmitted.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<IHasRequestId> CreateAccountUpdatesMultiObservable(string account = "", string modelCode = "", bool ledgerAndNLV = false) =>
        Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestAccountUpdatesMulti(id, account, modelCode, ledgerAndNLV),
                Request.CancelAccountSummary)
            .CacheSource(m => m switch
            {
                AccountUpdateMulti a => $"{a.Account} AccountValue: {a.Key}: {a.Currency}",
                _ => ""
            });

    public async Task<IList<AccountUpdateMulti>> GetAccountUpdatesMultiAsync(string account = "", string modelCode = "", bool ledgerAndNLV = false, CancellationToken ct = default) =>
        await CreateAccountUpdatesMultiObservable(account, modelCode, ledgerAndNLV)
            .TakeWhile(m => m is not AccountUpdateMultiEnd)
            .CastTo<AccountUpdateMulti>() // Throws for other types.
            .ToList()
            .ToTask(ct)
            .ConfigureAwait(false);

    /// <summary>
    /// Creates an observable which emits AccountSummary messages.
    /// All messages are sent initially, and then only updates. 
    /// AccountSummaryEnd indicates all the messages have been transmitted.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<IHasRequestId> CreateAccountSummaryObservable(string group = "All", params AccountSummaryTag[] tags) =>
        Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestAccountSummary(id, group, tags),
                Request.CancelAccountSummary)
            .CacheSource(m => m switch
            {
                AccountSummary s => $"{s.Account}: {s.Currency}, {s.Tag}",
                _ => ""
            });

    public async Task<IList<AccountSummary>> GetAccountSummaryAsync(string group = "All", CancellationToken ct = default, params AccountSummaryTag[] tags) =>
        await CreateAccountSummaryObservable(group, tags)
            .TakeWhile(m => m is not AccountSummaryEnd)
            .CastTo<AccountSummary>() // Throws for other types.
            .ToList()
            .ToTask(ct)
            .ConfigureAwait(false);

    /// <summary>
    /// Creates an observable which emits AccountPositionMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// AccountPositionMultiEnd indicates all the messages have been transmitted.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<IHasRequestId> CreateAccountPositionsMultiObservable(string account = "", string modelCode = "") =>
        Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestPositionsMulti(id, account, modelCode),
                Request.CancelAccountUpdatesMulti)
            .CacheSource(m => m switch
            {
                AccountPositionsMulti p => $"{p.Account}: {p.Contract.Stringify()}",
                _ => ""
            });

    public async Task<IList<AccountPositionsMulti>> GetAccountPositionsMultiAsync(string account = "", string modelCode = "", CancellationToken ct = default) =>
        await CreateAccountPositionsMultiObservable(account, modelCode)
            .TakeWhile(m => m is not AccountPositionsMultiEnd)
            .CastTo<AccountPositionsMulti>() // Throws for other types.
            .ToList()
            .ToTask(ct)
            .ConfigureAwait(false);
}
