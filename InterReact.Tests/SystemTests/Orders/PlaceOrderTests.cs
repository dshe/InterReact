using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Place : TestCollectionBase
{
    public Place(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task PlaceOrderTest()
    {
        if (!Client.Connection.RemoteIpEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        int orderId = Client.Request.GetNextId();

        Order order = new()
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        Task<IList<IHasOrderId>> task = Client
            .Response
            .OfType<IHasOrderId>()
            .Where(x => x.OrderId == orderId)
            .Take(TimeSpan.FromSeconds(5))
            .ToList()
            .ToTask();

        Client.Request.PlaceOrder(orderId, order, contract);

        IList<IHasOrderId> messages = await task;

        //Assert.Empty(messages.OfType<Alert>().Where(a => a.IsFatal));
  
        bool filled = messages.OfType<Execution>().Any();

        // sometimes the order will be filled
        Write("Order filled: " + filled);
    }
}
