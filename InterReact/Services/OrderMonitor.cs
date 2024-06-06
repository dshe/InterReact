using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Places an order and returns an OrderMonitor object (below) which can be used to monitor the order.
    /// </summary>
    public async Task<OrderMonitor> PlaceOrderAsync(Order order, Contract contract)
    {
        return await Task.Run(() =>
        {
            return new OrderMonitor(order, contract, Request, Response);
        });
    }

    public async Task<IList<Object>> RequestOpenOrdersAsync()
    {
        //todo: how to get return values from this.Response?
        Request.RequestOpenOrders();
        throw new NotImplementedException();
    }

    public async Task<IList<CompletedOrder>> GetCompleteOrdersAsync(bool api)
    {
        Task<IList<CompletedOrder>> task = 
            Response
            .OfType<CompletedOrder>()
            .Take(TimeSpan.FromMilliseconds(500))
            .ToList()
            .ToTask();

        Request.RequestCompletedOrders(api);

        var completedOrders = await task;
        return completedOrders;
    }
}

/// <summary>
/// This object is returned from Service.PlaceOrder().
/// It provides an observable which relays order messages: 
/// OpenOrder, OrderStatusReport, Execution, CommissionReport and possibly Alerts.
/// Results are cached and replayed to subscribers.
/// This observable completes only when the object is disposed.
/// Use Take(Timespan) operator to return an observable that contains the latest Values.
/// </summary>
public sealed class OrderMonitor : IDisposable
{
    private readonly ReplaySubject<IHasOrderId> Subject = new();
    private readonly Contract Contract;
    private readonly Request Request;
    public Order Order { get; }
    public int OrderId { get; }
    public IObservable<IHasOrderId> Messages { get; }

    internal OrderMonitor(Order order, Contract contract, Request request, IObservable<object> response)
    {
        Order = order;
        Contract = contract;
        Request = request;
        OrderId = Request.GetNextId();
        Messages = Subject.AsObservable();

        response
            .WithOrderId(OrderId)
            .Subscribe(Subject);

        Request.PlaceOrder(OrderId, Order, Contract);
    }

    public void ReplaceOrder() => Request.PlaceOrder(OrderId, Order, Contract);

    public void CancelOrder() => Request.CancelOrder(OrderId);

    public void Dispose() => Subject.Dispose();
}
