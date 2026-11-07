using System.Reactive.Linq;
namespace SystemTests;

public class OrderMonitoring(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    /*
    [Fact]
    public async Task OrderMonitorTestAsync()
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

        OrderMonitor orderMonitor = await Client.Service.PlaceOrderAsync(order, contract);

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        await orderMonitor.DisposeAsync();
    }

    [Fact]
    public async Task OrderMonitorCancellationTestAsync()
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

        OrderMonitor orderMonitor = await Client.Service.PlaceOrderAsync(order, contract);

        await orderMonitor.CancelOrderAsync();

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        await Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        await orderMonitor.DisposeAsync();
    }
        */

    [Fact]
    public async Task OrderMonitorModificationTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Cash,
            Symbol = "EUR",
            Currency = "USD",
            Exchange = "IDEALPRO"
        };

        IHasRequestId[] ticks = await Client.Service.GetMarketDataSnapshotAsync(contract, null, false, null, TestContext.Current.CancellationToken);

        double askPrice = ticks
            .OfTickClass(x => x.PriceTick)
            .Where(priceTick => priceTick.TickType == TickType.AskPrice)
            .Select(priceTick => priceTick.Price)
            .SingleOrDefault(0);

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
            LimitPrice = askPrice - .02 // should not execute
        };

        // Place the order
        OrderMonitor orderMonitor = await Client.Service.PlaceOrderAsync(order, contract);

        orderMonitor
            .Messages
            .Subscribe(m => Write($"OrderMonitor: {m.Stringify()}"));

        // Change the order
        orderMonitor.Order.LimitPrice = askPrice + .02; // should execute
        // Resubmit the changed order
        await orderMonitor.ReplaceOrderAsync();

        // Wait for execution
        Execution? execution = await orderMonitor
            .Messages
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(5))
            .FirstOrDefaultAsync();

        await orderMonitor.DisposeAsync();
    }
}
