namespace InterReact;

public partial class Service
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
            .ToObservableContinuousWithRequestId(
                Request.GetNextRequestId,
                requestId => Request.RequestAccountSummary(requestId), Request.CancelAccountSummary)
            .CacheSource(GetAccountSummaryCacheKey);
    }

    private static string GetAccountSummaryCacheKey(IHasRequestId m)
    {
        return m switch
        {
            AccountSummary a => $"{a.Account}+{a.Currency}+{a.Tag}",
            AccountSummaryEnd => "AccountSummaryEnd",
            Alert _ => "",
            _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
        };
    }
}
