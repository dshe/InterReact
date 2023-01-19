using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Sucessive ids used for requests and orders are generated using "Request.GetNextId()".
    /// In case there are multiple client applcations connected to an account
    /// which are creating orders, the next id may be updated(incresed) using UpdateNextIdAsync().
    /// https://interactivebrokers.github.io/tws-api/order_submission.html
    /// </summary>
    public async Task UpdateNextIdAsync()
    {
        Task<NextOrderId> task = Response.OfType<NextOrderId>().FirstAsync().ToTask();
        Request.RequestNextOrderId();
        await task.ConfigureAwait(false);
        // The result is updated in the NextOrderId response object constructor
    }
}
