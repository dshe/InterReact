using NodaTime.Text;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    private static readonly InstantPattern Pattern = InstantPattern.CreateWithInvariantCulture("S");

    /// <summary>
    /// Creates an observable which continually emits AccountSummary objects.
    /// Objects: AccountSummary, AccountSummaryEnd, Alert.
    /// The complete account summary is sent initially, and then only any changes.
    /// AccountSummaryEnd is emitted after the initial values for each account have been emitted.
    /// The latest values are cached for replay to new subscribers.
    /// Multiple subscribers are supported.
    /// </summary>
    public IObservable<object> AccountSummaryObservable { get; }

    private IObservable<object> CreateAccountSummaryObservable()
    {
        return Response
            .ToObservableContinuousWithId(
                Request.GetNextId,
                id => Request.RequestAccountSummary(id),
                Request.CancelAccountSummary)
            .CacheSource(GetAccountSummaryCacheKey);
    }

    private static string GetAccountSummaryCacheKey(object m)
    {
        return m switch
        {
            AccountSummary a => $"{a.Account}+{a.Currency}+{a.Tag}",
            AccountSummaryEnd => "AccountSummaryEnd",
            Alert alert => Pattern.Format(alert.Time),
            _ => throw new ArgumentException($"Unhandled type: {m.GetType()}.")
        };
    }
}
