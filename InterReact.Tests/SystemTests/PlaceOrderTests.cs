using System.Reactive.Linq;
namespace SystemTests;

public class Place(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task PlaceOrderTestAsync()
    {
        if (!Client.RemoteIpEndPoint.IsTwsDemoAccountPort())
            throw new InvalidOperationException("Demo account is required since an order will be placed. Please first login to the TWS demo account.");

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

        await Client.Request.PlaceOrderAsync(orderId, order, contract);

        await Task.Delay(TimeSpan.FromSeconds(7), TestContext.Current.CancellationToken);

        await Client.Request.CancelOrderAsync(orderId);
    }
}

