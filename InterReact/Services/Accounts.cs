namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits AccountUpdateMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// After sending all initial messages, a message with property "AccountUpdateMulti.IsEndMessage"=true is sent.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<AccountUpdateMulti> CreateAccountUpdatesMultiObservable(string account = "", string modelCode = "", bool ledgerAndNLV = false) =>
        Response
            .ToObservableWithId<AccountUpdateMulti>(
                Request.GetNextId,
                id => Request.RequestAccountUpdatesMulti(id, account, modelCode, ledgerAndNLV),
                Request.CancelAccountUpdatesMulti)
            .CacheSource(m => $"{m.Account} {m.ModelCode} {m.Key} {m.Currency}", m => m.IsEndMessage);

    public async Task<AccountUpdateMulti[]> GetAccountUpdatesMultiAsync(string account = "", string modelCode = "", bool ledgerAndNLV = false, TimeSpan? timeout = null) =>
        await Response.ToObservableWithId<AccountUpdateMulti>(
                Request.GetNextId,
                id => Request.RequestAccountUpdatesMulti(id, account, modelCode, ledgerAndNLV),
                Request.CancelAccountUpdatesMulti,
                m => m is AccountUpdateMulti a && a.IsEndMessage)
            .ToArray()
            .Timeout(timeout ?? TimeSpan.MaxValue);


    /// <summary>
    /// Creates an observable which emits AccountPositionMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// After sending all initial messages, a message with property "AccountPositionMulti.IsEndMessage"=true is sent.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<AccountPositionsMulti> CreateAccountPositionsMultiObservable(string account = "", string modelCode = "") =>
        Response
            .ToObservableWithId<AccountPositionsMulti>(
                Request.GetNextId,
                id => Request.RequestPositionsMulti(id, account, modelCode),
                Request.CancelAccountUpdatesMulti)
            .CacheSource(m => $"{m.Account} {m.ModelCode} {m.Contract.ContractId}", m => m.IsEndMessage);

    public async Task<AccountPositionsMulti[]> GetAccountPositionsMultiAsync(string account = "", string modelCode = "", TimeSpan? timeout = null) =>
        await Response.ToObservableWithId<AccountPositionsMulti>(
                Request.GetNextId,
                id => Request.RequestPositionsMulti(id, account, modelCode),
                Request.CancelAccountUpdatesMulti,
                m => m is AccountPositionsMulti a && a.IsEndMessage)
            .ToArray()
            .Timeout(timeout ?? TimeSpan.MaxValue);
}
