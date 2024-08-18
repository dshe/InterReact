using Stringification;
using System.Reactive.Linq;

namespace Orders;

public class Monitor(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task OrderMonitorTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Cash,
            Symbol = "EUR",
            Currency = "USD",
            Exchange = "IDEALPRO"
        };

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 50000,
            OrderType = OrderTypes.Market
        };

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        await Task.Delay(TimeSpan.FromSeconds(5));

        orderMonitor.Dispose();
    }

    [Fact]
    public async Task OrderMonitorCancellationTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Cash,
            Symbol = "USD",
            Currency = "SGD",
            Exchange = "IDEALPRO"
        };

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 60000,
            OrderType = OrderTypes.Market
        };

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);

        orderMonitor.CancelOrder();

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Dispose();
    }

    [Fact]
    public async Task OrderMonitorModificationTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Cash,
            Symbol = "CHF",
            Currency = "USD",
            Exchange = "IDEALPRO"
        };

        double askPrice = await Client
            .Service
            .CreateMarketDataSnapshotObservable(contract)
            .OfTickClass(x => x.PriceTick)
            .Where(priceTick => priceTick.TickType == TickType.AskPrice)
            .Select(priceTick => priceTick.Price)
            .Take(TimeSpan.FromSeconds(3))
            .FirstOrDefaultAsync();

        if (askPrice <= 0)
        {
            Write("Price not available.");
            return;
        }

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 40000,
            OrderType = OrderTypes.Limit,
            LimitPrice = askPrice - .01 // should not execute
        };

        // Place the order
        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        // Change the order
        orderMonitor.Order.LimitPrice = askPrice + .01; // should execute
        // Resubmit the changed order
        orderMonitor.ReplaceOrder();

        // Wait for execution
        Execution? execution = await orderMonitor
            .Messages
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(3))
            .FirstOrDefaultAsync();

        orderMonitor.Dispose();

        //Assert.NotNull(execution);
    }
}
