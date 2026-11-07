using System.Reactive.Threading.Tasks;
namespace InterReact;

public partial class Service
{
    public async Task<IList<string>> GetManagedAccountsAsync(CancellationToken ct = default)
    {
        Task<ManagedAccounts> task = _response
                .OfType<ManagedAccounts>()
                .FirstAsync()
                .ToTask(ct);

        await _request.RequestManagedAccountsAsync().ConfigureAwait(false);

        ManagedAccounts managedAccounts = await task.ConfigureAwait(false);

        string[] accounts = managedAccounts.Accounts.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return accounts;
    }
}
