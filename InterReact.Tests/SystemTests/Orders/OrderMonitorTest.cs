using Stringification;
using System.Reactive.Linq;

namespace Orders;

public class Monitor : TestCollectionBase
{
    public Monitor(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OrderMonitorTest()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
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
  
        orderMonitor.Messages.Subscribe(m => Write(m.Stringify()));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Dispose();
    }

    [Fact]
    public async Task OrderMonitorCancellationTest()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
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

        IList<OrderStatusReport> reports = await orderMonitor
            .Messages
            .OfType<OrderStatusReport>()
            .Take(TimeSpan.FromSeconds(2))
            .ToList();

        Assert.True(reports.Where(x => 
            x.Status == OrderStatus.Cancelled || x.Status == OrderStatus.ApiCancelled)
                .Any());
      
        orderMonitor.Dispose();
    }
}
