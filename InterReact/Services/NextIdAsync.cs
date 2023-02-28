using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Sucessive ids used for requests and orders are generated using "Request.GetNextId()".
    /// In case there are multiple client applications connected to an account
    /// which are creating orders, the next id may be updated(increased) using UpdateNextIdAsync().
    /// https://interactivebrokers.github.io/tws-api/order_submission.html
    /// </summary>
    public async Task UpdateNextIdAsync(CancellationToken ct = default)
    {
        Task<NextOrderId> task = Response
            .OfType<NextOrderId>()
            .FirstAsync()
            .ToTask(ct);
        
        Request.RequestNextOrderId();
        
        await task.ConfigureAwait(false);
        // The result is updated in the message NextOrderId response object constructor.
    }
}
