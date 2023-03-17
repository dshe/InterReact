using Stringification;
using System.Reactive.Linq;

namespace Orders;

public class Monitor : TestCollectionBase
{
    public Monitor(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OrderMonitorTest()
    {
        if (!Client.Connection.RemoteIpEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "TSLA",
            Currency = "USD",
            Exchange = "SMART"
        };

        Order order = new()
        {
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        OrderMonitor orderMonitor = Client
            .Service
            .PlaceOrder(order, contract);
  
        orderMonitor
            .Messages
            .Subscribe(m => Write(m.Stringify()));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Dispose();
    }

    [Fact]
    public async Task OrderMonitorCancellationTest()
    {
        if (!Client.Connection.RemoteIpEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "GOOG",
            Currency = "USD",
            Exchange = "SMART"
        };

        Order order = new()
        {
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        OrderMonitor orderMonitor = Client
            .Service
            .PlaceOrder(order, contract);

        orderMonitor.CancelOrder();

        IList<OrderStatusReport> reports = await orderMonitor
            .Messages
            .OfType<OrderStatusReport>()
            .Take(TimeSpan.FromSeconds(2))
            .ToList();

        Assert.True(reports
            .Any(x => x.Status == OrderStatus.Cancelled || x.Status == OrderStatus.ApiCancelled));
      
        orderMonitor.Dispose();
    }

    [Fact]
    public async Task OrderMonitorModifyTest()
    {
        if (!Client.Connection.RemoteIpEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "IBKR",
            Currency = "USD",
            Exchange = "SMART"
        };

        PriceTick? askPriceTick = await Client
            .Service
            .CreateTickSnapshotObservable(contract)
            .OfTickClass(msg => msg.PriceTick)
            .FirstOrDefaultAsync(priceTick =>
                priceTick.TickType == TickType.AskPrice || priceTick.TickType == TickType.DelayedAskPrice);

        Assert.NotNull(askPriceTick);
        double askPrice = askPriceTick.Price;

        Order order = new()
        {
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Limit,
            LimitPrice = askPrice - 1 // should not execute
        };

        // Place the order
        OrderMonitor monitor = Client.Service.PlaceOrder(order, contract);

        await Task.Delay(500);

        // Change then resubmit the order
        monitor.Order.LimitPrice = askPrice + 1; // should execute
        monitor.ReplaceOrder();

        Execution? execution = await monitor
            .Messages
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(2))
            .FirstOrDefaultAsync();

        Assert.NotNull(execution);
        Write("Order filled");
    }

}
