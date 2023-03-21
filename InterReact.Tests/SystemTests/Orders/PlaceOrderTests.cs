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
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        Task<Execution?> task = Client
            .Response
            .WithOrderId(orderId)
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(5))
            .FirstOrDefaultAsync()
            .ToTask();

        Client.Request.PlaceOrder(orderId, order, contract);

        Execution? execution = await task;

        Assert.NotNull(execution);
    }
}
