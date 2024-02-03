using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Place : CollectionTestBase
{
    public Place(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task PlaceOrderTest()
    {
        if (!Client.RemoteIpEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        int orderId = Client.Request.GetNextId();

        Order order = new()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderTypes.Market
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
