using System.Diagnostics.Contracts;
using System.Reactive.Subjects;
namespace InterReact;

public partial class Service : IDisposable
{
    /// <summary>
    /// Places an order and returns an OrderMonitor object (below) which can be used to monitor the order.
    /// </summary>
    public async ValueTask<OrderMonitor> PlaceOrderAsync(Order order, Contract contract) =>
        await OrderMonitor.PlaceOrderAsync(order, contract, _request, _response).ConfigureAwait(false);
}

/// <summary>
/// This object is returned from Service.PlaceOrder().
/// It provides an observable which relays order messages: 
/// OpenOrder, OrderStatusReport, Execution, CommissionReport and possibly Alerts.
/// Results are cached and replayed to subscribers.
/// This observable completes when the object is disposed.
/// Use Take(Timespan) operator to return an observable that contains the latest Values.
/// </summary>
public sealed class OrderMonitor : IAsyncDisposable
{
    private readonly ReplaySubject<IHasOrderId> _subject = new();
    private readonly Request _request;
    private readonly Contract _contract;
    public Order Order { get; }
    public int OrderId { get; }
    public IObservable<IHasOrderId> Messages { get; }

    internal OrderMonitor(Order order, Contract contract, Request request, IObservable<object> response)
    {
        Order = order;
        _contract = contract;
        _request = request;
        OrderId = _request.GetNextId();
        Order.OrderId = OrderId;
        Messages = _subject.AsObservable();

        response
            .WithOrderId(OrderId)
            .Subscribe(_subject);
    }

    internal static async ValueTask<OrderMonitor> PlaceOrderAsync(Order order, Contract contract, Request request, IObservable<object> response)
    {
        OrderMonitor orderMonitor = new(order, contract, request, response);
        await request.PlaceOrderAsync(orderMonitor.OrderId, order, contract).ConfigureAwait(false);
        return orderMonitor;
    }

    public async ValueTask ReplaceOrderAsync() => await _request.PlaceOrderAsync(OrderId, Order, _contract).ConfigureAwait(false);

    public async ValueTask CancelOrderAsync() => await _request.CancelOrderAsync(OrderId).ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        await CancelOrderAsync().ConfigureAwait(false);
        _subject.Dispose();
    }
}
