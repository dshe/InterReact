namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits AccountUpdateMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// After sending all initial messages, a message with property "AccountUpdateMulti.IsEndMessage"=true is sent.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public IObservable<IHasRequestId> CreateAccountUpdatesMultiObservable(string account = "ALL", string modelCode = "", bool ledgerAndNLV = false) =>
        _response
            .ToObservableWithId(
                _request.GetNextId,
                id => _request.RequestAccountUpdatesMultiAsync(id, account, modelCode, ledgerAndNLV),
                _request.CancelAccountUpdatesMultiAsync)
            .CacheSource(m => m switch
            {
                AccountUpdateMulti a => $"{a.Account}:{a.ModelCode}:{a.Key}:{a.Currency}",
                _ => ""
            }, m => m is AccountUpdateMulti a && a.IsEndMessage);

    public async Task<AccountUpdateMulti[]> GetAccountUpdatesMultiAsync(string account = "ALL", string modelCode = "", bool ledgerAndNLV = false, TimeSpan? timeout = null, CancellationToken ct = default)
    {
        string errorMessage = await VerifyAccountArgAsync(account, timeout).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(errorMessage))
            throw new ArgumentException(errorMessage);

        return await _response
            .ToObservableWithId(
                _request.GetNextId,
                id => _request.RequestAccountUpdatesMultiAsync(id, account, modelCode, ledgerAndNLV),
                _request.CancelAccountUpdatesMultiAsync)
            .OfTypeOnly<AccountUpdateMulti>()
            .TakeUntil(m => m.IsEndMessage)
            .WithTimeout(timeout, ct)
            .ToArray();
    }

    public async Task<string> VerifyAccountArgAsync(string account, TimeSpan? timeout = null)
    {
        if (string.Equals(account, "ALL", StringComparison.OrdinalIgnoreCase))
            return "";

        IReadOnlyList<string> accounts = await GetManagedAccountsAsync(timeout).ConfigureAwait(false);

        if (string.IsNullOrEmpty(account))
            return (accounts.Count == 1) ? "" : "Since there are multiple accounts, an account must be specified.";

        return accounts.Contains(account, StringComparer.OrdinalIgnoreCase) ? "" : $"Account '{account}' is not found.";
    }

}
