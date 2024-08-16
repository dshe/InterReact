using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Place(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task PlaceOrderTest()
    {
        if (!Client.RemoteIpEndPoint.IsUsingIBDemoPort())
            throw new Exception("Use demo account to place order.");

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

        Task<IHasOrderId> task = Client.Response
            .WithOrderId(orderId)
            .FirstAsync()
            .ToTask();

        Client.Request.PlaceOrder(orderId, order, contract);

        await task;
        await Task.Delay(TimeSpan.FromSeconds(3));
    }
}
