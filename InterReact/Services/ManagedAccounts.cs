using System.Diagnostics;
namespace InterReact;

public partial class Service
{
    private readonly SemaphoreSlim _managedAccountsSemaphore = new(1, 1);

    public async Task<IReadOnlyList<string>> GetManagedAccountsAsync(TimeSpan? timeout = null, CancellationToken ct = default)
    {
        await _managedAccountsSemaphore.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (_options.ManagedAccounts.Count == 0)
            {
                // Options.ManagedAccounts is updated whenever a ManagedAccounts message is received.
                await _response
                    .OfType<ManagedAccounts>()
                    .ToObservable<ManagedAccounts>(_request.RequestManagedAccountsAsync)
                    .Select(x => x.Accounts)
                    .TakeUntil(ct)
                    .Timeout(timeout ?? TimeSpan.MaxValue)
                    .SingleAsync();
            }

            Debug.Assert(_options.ManagedAccounts.Count > 0);

            return _options.ManagedAccounts;
        }
        finally
        {
            _managedAccountsSemaphore.Release();
        }
    }
}
