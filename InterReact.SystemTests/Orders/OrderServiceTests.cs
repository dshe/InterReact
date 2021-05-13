using System.Threading.Tasks;
using InterReact;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;

namespace SystemTests.Orders
{
    public class OrderServiceTests : BaseTest
    {
        public OrderServiceTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestPlaceOrder()
        {
            if (!Client.Config.IsDemoAccount)
                return;

            //var order = new Order { TradeAction = TradeAction.Buy, TotalQuantity = 200, OrderType = OrderTypes.Limit, LimitPrice = 100 };
            //order.GoodTillDate = "20161226 15:59:00 EST";
            //order.MinimumQuantity = 100;
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };
            var orderId = Client.Request.GetNextId();

            await Task.Delay(1000);

            //Client.Request.PlaceOrder(orderId, order, contract);

            await Task.Delay(10000);

            Client.Request.CancelOrder(orderId);

            await Task.Delay(3000);
        }


        [Fact]
        public async Task TestOpenOrder()
        {
            //var order = new Order { TradeAction = TradeAction.Buy, TotalQuantity = 200, OrderType = OrderTypes.Limit, LimitPrice = 100 };
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };

            var orderId = Client.Request.GetNextId();
            //var orderId = ;

            //order.GoodTillDate = "20161226 15:59:00 EST";
            //order.MinimumQuantity = 100;

            //await Task.Delay(2000);

            //Client.Request.PlaceOrder(orderId, order, contract);
            //await Task.Delay(2000);

            //var openOrders = await Client.Services.OpenOrdersObservable().ToList().Timeout(TimeSpan.FromSeconds(10));

            await Task.Delay(1000);

            //Client.Request.CancelOrder(orderId);
        }


        [Fact]
        public async Task TestOrder()
        {
            //var openOrders = await Client.Services.OpenOrdersObservable().ToList();
            //var executionAndCommissions = await Client.Services.ExecutionAndCommissionsObservable.ToList();

            //Client.Request.RequestExecutions(42);


            //await Client.Services.OrderMessagesObservablex;
            //await Client.Services.OrderMessagesObservableCreatex();


            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "SPY", Currency = "USD", Exchange = "SMART" };
            //var order = new Order { TradeAction = TradeAction.Buy, TotalQuantity = 100, OrderType = OrderTypes.Market };

            //Client.Request.PlaceOrder(123, order, contract);

            //var orderMonitor = Client.Services.PlaceOrder(order, contract, 123);
            //var xxx = await orderMonitor.MessagesObservable.Take(TimeSpan.FromSeconds(10)).ToList();

            // CAUTION CAUTION CAUTION: This places an order!
            //var xorder = Client.Services.PlaceOrder(contract, order);

            //await Task.Delay(1000);
            //xorder.OrderEventsObservable.Subscribe(x => WriteMessageTo(x.Stringify()), e => WriteMessageTo("Exception: " + e));
            //Client.Request.RequestExecutions(123);
            //await Task.Delay(10000);
            //;

            await Task.Delay(3000);
            ;
        }


    }
}
