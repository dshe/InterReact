using Stringification;
using System.Reactive.Linq;

namespace Orders;

public class Place(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture, true)
{
    [Fact]
    public async Task PlaceOrderTest()
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
            TotalQuantity = 1,
            OrderType = OrderTypes.Market
        };

        int orderId = Client.Request.GetNextId();

        Client
            .Response
            .WithOrderId(orderId)
            .Subscribe(m => Write($"Message: {m.Stringify()}"));

        Client.Request.PlaceOrder(orderId, order, contract);

        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}
