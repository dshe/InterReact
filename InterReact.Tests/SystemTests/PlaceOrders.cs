using System.Reactive.Linq;
namespace SystemTests;

public class PlaceOrders(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task PlaceOrderTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Commodity,
            Symbol = "XAUUSD",
            Currency = "USD",
            Exchange = "SMART"
        };

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 5,
            OrderType = OrderTypes.Market,
        };

        int orderId = Client.Request.GetNextId();

        IDisposable sub = Client.Response.Subscribe(m => Write($"Message: {m.Stringify()}"));

        await Client.Request.PlaceOrderAsync(orderId, order, contract);

        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        sub.Dispose();
    }

    [Fact]
    public async Task ListOrdersAsync()
    {
        IDisposable sub = Client.Response.Subscribe(m => Write($"Message: {m.Stringify()}"));

        await Client.Request.RequestCompletedOrdersAsync(false);
        await Client.Request.RequestExecutionsAsync(345);

        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        sub.Dispose();
    }
}
