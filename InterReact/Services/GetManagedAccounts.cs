using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns the list of managed accounts.
    /// TWS does not support more than one concurrent request.
    /// </summary>
    public async Task<IList<string>> GetManagedAccountsAsync(CancellationToken ct)
    {
        Task<List<string>> task = Response
            .OfType<ManagedAccounts>()
            .Select(x => x.Accounts.ToList())
            .FirstAsync()
            .ToTask(ct);

        Request.RequestManagedAccounts();

        return await task.ConfigureAwait(false);
    }
}
