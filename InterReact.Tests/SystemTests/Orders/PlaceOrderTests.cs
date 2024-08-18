using Stringification;
using System.Reactive.Linq;

namespace Orders;

public class Place(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task PlaceOrderTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 100,
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
