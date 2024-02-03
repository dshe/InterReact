namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits AccountSummary objects for all accounts.
    /// All positions are sent initially, and then only updates. 
    /// The latest values are cached for replay to new subscribers.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public IObservable<AccountSummary> AccountSummaryObservable { get; }

    private IObservable<AccountSummary> CreateAccountSummaryObservable()
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestAccountSummary(id),
                Request.CancelAccountSummary)
            .AlertMessageToError()
            .OfType<AccountSummary>()
            .CacheSource(a => $"{a.Account}: {a.Currency}, {a.Tag}");
    }
}
