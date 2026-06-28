namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits AccountPosition messages.
    /// All messages are sent initially, and then only updates.
    /// After sending all initial messages, an empty message with property "AccountPosition.IsEndMessage"=true is sent.
    /// The latest messages are cached for replay to new subscribers.
    /// .CacheSource(a => $"{a.Account}:{a.Contract.ContractId}", a => a.IsEndMessage);
    /// </summary>
    public IObservable<AccountPosition> AccountPositionsObservable { get; }

    private IObservable<AccountPosition> CreateAccountPositionsObservable() =>
        _response
            .ToObservable<AccountPosition>(_request.RequestPositionsAsync, _request.CancelPositionsAsync)
            .CacheSource(a => $"{a.Account}:{a.Contract.ContractId}", a => a.IsEndMessage, true);

    public async Task<AccountPosition[]> GetAccountPositionsAsync(TimeSpan? timeout = null, CancellationToken ct = default) =>
        await AccountPositionsObservable
            .TakeWhile(a => !a.IsEndMessage)
            .TakeUntil(ct)
            .Timeout(timeout ?? TimeSpan.MaxValue)
            .ToArray();
}
