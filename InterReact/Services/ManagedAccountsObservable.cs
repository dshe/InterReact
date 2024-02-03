namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Observable which, upon subscription, returns the list of managed accounts.
    /// TWS does not support more than one concurrent request.
    /// </summary>
    public IObservable<IList<string>> ManagedAccountsObservable { get; }

    private IObservable<IList<string>> CreateManagedAccountsObservable()
    {
        return Response
            .ToObservableSingle<ManagedAccounts>(Request.RequestManagedAccounts)
            .Select(x => x.Accounts.ToList())
            .ShareSource();
    }
}
