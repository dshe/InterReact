using InterReact;
using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders
{
    public class OrderIntegrationTests: CollectionTestBase
    {

        public OrderIntegrationTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }


        [Fact]
        public async Task Place_GetOpen_Cancel_Order_Test()
        {
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

            orderMonitor.CancelOrder();

            OrderStatusReport report = await orderMonitor
                .Messages
                .OfType<OrderStatusReport>()
                .Take(TimeSpan.FromSeconds(3))
                .FirstOrDefaultAsync();

            //orderMonitor.Dispose();
            Assert.True(report.Status == OrderStatus.Cancelled || report.Status == OrderStatus.ApiCancelled);


            // 等待1秒
            await Task.Delay(1000);


            // Get the order, if contains orderId, succeed
            Task<IList<Object>> task2 = Client
                .Response
                .Take(TimeSpan.FromSeconds(3))
                .ToList<Object>()
                .ToTask();

            Client.Request.RequestOpenOrders();
            var openOrder = await task2;
            Assert.IsType<Order>(openOrder);

            // 等待1秒
            await Task.Delay(1000);


            // cancel the order
            Task<Execution?> task3 = Client
            .Response
            .WithOrderId(orderId)
            .OfType<Execution>()
            .Take(TimeSpan.FromSeconds(3))
            .FirstOrDefaultAsync()
            .ToTask();
            Client.Request.CancelOrder(orderId);
            Execution? execution3 = await task3;
            Assert.NotNull(execution3);

            await Task.Run(() => Console.WriteLine("3"));
        }

       
    }
}
