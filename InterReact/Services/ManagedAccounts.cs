namespace InterReact;

public partial class Service
{
    public IObservable<string[]> CreateManagedAccountsObservable()
    {
        IObservable<string[]> observable = Response
            .OfType<ManagedAccounts>()
            .FirstAsync()
            .Select(x => x.Accounts.Split(',', StringSplitOptions.RemoveEmptyEntries));

        Request.RequestManagedAccounts();

        return observable;
    }

    public async Task<string[]> GetManagedAccountsAsync(TimeSpan? timeout = null) =>
        await CreateManagedAccountsObservable()
            .Timeout(timeout ?? TimeSpan.MaxValue);
}
