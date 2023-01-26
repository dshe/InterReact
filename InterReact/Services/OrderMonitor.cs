using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Places an order and returns an OrderMonitor object which can be used to monitor the order.
    /// </summary>
    public OrderMonitor PlaceOrder(Order order, Contract contract) =>
        new(Request, Response, order, contract);
}

/// <summary>
/// This object is returned from Services.PlaceOrder().
/// It provides an observable which relays order messages (OpenOrder, OrderStatusReport, Execution, CommissionReport and possibly, Alerts).
/// Results are cached and replayed to subscribers.
/// This observable completes only when the object is disposed.
/// Use Take(Timespan) operator to return an observable that contains the latest Values.
/// </summary>
public sealed class OrderMonitor : IDisposable
{
    private readonly Request Request;
    private readonly ReplaySubject<object> subject = new();
    private readonly Contract Contract;
    public Order Order { get; private set; }
    public int OrderId { get; }
    public IObservable<object> Messages => subject.AsObservable();

    internal OrderMonitor(Request request, IObservable<object> response, Order order, Contract contract)
    {
        Request = request;
        Order = order;
        Contract = contract;
        OrderId = Request.GetNextId();

        response
            .OfType<IHasOrderId>()
            .Where(m => m.OrderId == OrderId)
            .Subscribe(subject);

        Request.PlaceOrder(OrderId, Order, Contract);
    }

    public void ModifyOrder(Order order)
    {
        Order = order;
        Request.PlaceOrder(OrderId, Order, Contract);
    }
 
    public void CancelOrder() => Request.CancelOrder(OrderId);
 
    public void Dispose() => subject.Dispose();
}
