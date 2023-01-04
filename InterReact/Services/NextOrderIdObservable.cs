using System.Reactive.Linq;
using System.Threading;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Sucessive order ids are generated using "Request.GetNextOrderId()".
    /// However, in case there are multiple client applcations connected to an account
    /// which are creating orders, the next order id should be obtained using:
    /// int orderId = await NextOrderIdObservable;
    /// https://interactivebrokers.github.io/tws-api/order_submission.html
    /// </summary>
    public IObservable<int> NextOrderIdObservable { get; }

    private IObservable<int> CreateNextOrderIdObservable() =>
        Response
            .ToObservableSingle<NextOrderId>(() => Request.RequestNextOrderId())
            .Select(m => m.OrderId)
            .Do(orderId => Interlocked.Exchange(ref Request.NextOrderId, orderId))
            .ShareSource();
}
