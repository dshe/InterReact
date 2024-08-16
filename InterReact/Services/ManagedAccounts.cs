using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a list of managed accounts.
    /// </summary>
    public async Task<IList<string>> GetManagedAccountsAsync(CancellationToken ct = default)
    {
        Task<string[]> task = Response
            .OfType<ManagedAccounts>()
            .Select(x => x.Accounts.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .FirstAsync()
            .ToTask(ct);

        Request.RequestManagedAccounts();

        return await task.ConfigureAwait(false);
    }
}
