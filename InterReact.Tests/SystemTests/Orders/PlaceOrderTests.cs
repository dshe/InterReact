using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class PlaceOrders : TestCollectionBase
{
    public PlaceOrders(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task PlaceMarketOrderTest()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        var orderId = Client.Request.GetNextId();

        var task = Client.Response
            .OfType<Execution>()
            .Where(x => x.OrderId == orderId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        var order = new Order
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        Client.Request.PlaceOrder(orderId, order, StockContract1);

        await task;
    }

    [Fact]
    public async Task PlaceLimitOrderTest()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        int orderId = Client.Request.GetNextId();
        int requestId = Client.Request.GetNextId();

        PriceTick timeoutIndicator = new();

        // find the price
        Task<PriceTick> taskPrice = Client.Response
            .OfType<PriceTick>()
            .Where(x => x.RequestId == requestId)
            .Where(x => x.TickType == TickType.AskPrice)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(10), Observable.Return(timeoutIndicator))
            .ToTask();

        Client.Request.RequestMarketData(requestId, StockContract1, null, isSnapshot: true);

        var priceTick = await taskPrice;

        if (priceTick == timeoutIndicator)
        {
            Write("Timeout waiting for AskPrice.");
            return;
        }

        // place the order
        var taskOpenOrder = Client.Response
            .OfType<OpenOrder>()
            .Where(x => x.OrderId == orderId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        var order = new Order
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Limit,
            LimitPrice = priceTick.Price - 1 // below market
        };

        Client.Request.PlaceOrder(orderId, order, StockContract1);

        await taskOpenOrder;

        // cancel the order
        var taskCancelled = Client.Response
            .OfType<OrderStatusReport>()
            .Where(x => x.OrderId == orderId)
            .Where(x => x.Status == OrderStatus.Cancelled)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        Client.Request.CancelOrder(orderId);

        await taskCancelled;
    }

}

