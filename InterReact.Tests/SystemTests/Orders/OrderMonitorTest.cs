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

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);
  
        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        Execution? execution = await orderMonitor
            .Messages
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(5))
            .FirstOrDefaultAsync();

        orderMonitor.Dispose();

        Assert.NotNull(execution);
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

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);

        orderMonitor.CancelOrder();

        OrderStatusReport report = await orderMonitor
            .Messages
            .OfType<OrderStatusReport>()
            .Take(TimeSpan.FromSeconds(3))
            .FirstOrDefaultAsync();

        orderMonitor.Dispose();

        Assert.True(report.Status == OrderStatus.Cancelled || report.Status == OrderStatus.ApiCancelled);
    }

    [Fact]
    public async Task OrderMonitorModificationTest()
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
        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);

        //await Task.Delay(100);

        // Change the order
        orderMonitor.Order.LimitPrice = askPrice + 1; // should execute
        // Resubmit the c hanged order
        orderMonitor.ReplaceOrder();

        // Wait for execution
        Execution? execution = await orderMonitor
            .Messages
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(2))
            .FirstOrDefaultAsync();

        orderMonitor.Dispose();

        Assert.NotNull(execution);
    }
}
