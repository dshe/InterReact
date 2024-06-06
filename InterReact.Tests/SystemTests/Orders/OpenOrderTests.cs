using Stringification;

namespace Orders;

public class Open : CollectionTestBase
{
    public Open(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OpenOrdersAsyncTest()
    {
        await Task.Delay(1000);

        // 1 place order
        // Place the order with orderId: orderId
        var contract = new Contract()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        int orderId = Client.Request.GetNextId();

        var order = new Order()
        {
            Action = OrderAction.Buy,
            TotalQuantity = 1,
            OrderType = OrderTypes.Limit,
            LimitPrice = 1,
            TimeInForce = OrderTimeInForce.GoodUntilCancelled
        };

        OrderMonitor orderMonitor = await Client.Service.PlaceOrderAsync(order, contract);

        //2 Request Open Order
        IList<OpenOrder> openOrders = await Client
            .Service
            .RequestOpenOrdersAsync()
            .WaitAsync(TimeSpan.FromSeconds(1));

        Write($"Open orders found: {openOrders.Count}.");

        foreach (Object item in openOrders)
            Write(item.Stringify());

        //3 cancel Order
        orderMonitor.CancelOrder();
        orderMonitor.Dispose();
    }
}

