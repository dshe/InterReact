namespace InterReact;

public partial class Svc
{
    /// <summary>
    /// Creates an observable which continually emits AccountSummary objects.
    /// The complete account summary is sent initially, and then only any changes.
    /// AccountSummaryEnd is emitted after the initial values for each account have been emitted.
    /// Multiple subscribers are supported. The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<IHasRequestId> AccountSummaryObservable { get; }

    private IObservable<IHasRequestId> CreateAccountSummaryObservable()
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestAccountSummary(id), Request.CancelAccountSummary)
            .CacheSource(GetAccountSummaryCacheKey);
    }

    private static string GetAccountSummaryCacheKey(IHasRequestId o)
    {
        return o switch
        {
            AccountSummary a => $"{a.Account}+{a.Currency}+{a.Tag}",
            AccountSummaryEnd => "AccountSummaryEnd",
            Alert _ => "",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}
