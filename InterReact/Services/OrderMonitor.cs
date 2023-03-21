using System.Reactive.Subjects;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Places an order and returns an OrderMonitor object which can be used to monitor the order.
    /// </summary>
    public OrderMonitor PlaceOrder(Order order, Contract contract) =>
        new(order, contract, Request, Response);
}

/// <summary>
/// This object is returned from Service.PlaceOrder().
/// It provides an observable which relays order messages: 
/// OpenOrder, OrderStatusReport, Execution, CommissionReport and possibly, Alerts.
/// Results are cached and replayed to subscribers.
/// This observable completes only when the object is disposed.
/// Use Take(Timespan) operator to return an observable that contains the latest Values.
/// </summary>
public sealed class OrderMonitor : IDisposable
{
    private readonly ReplaySubject<IHasOrderId> subject = new();
    private readonly Request Request;
    private readonly Contract Contract;
    public Order Order { get; }
    public int OrderId { get; }
    public IObservable<IHasOrderId> Messages { get; }

    internal OrderMonitor(Order order, Contract contract, Request request, IObservable<object> response)
    {
        Contract = contract;
        Order = order;
        Request = request;
        OrderId = Request.GetNextId();
        Messages = subject.AsObservable();

        response
            .WithOrderId(OrderId)
            .Subscribe(subject);

        Request.PlaceOrder(OrderId, Order, Contract);
    }

    public void ReplaceOrder() => Request.PlaceOrder(OrderId, Order, Contract);
 
    public void CancelOrder() => Request.CancelOrder(OrderId);
 
    public void Dispose() => subject.Dispose();
}
