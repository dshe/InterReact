using InterReact;
using InterReact.SystemTests;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Stringification;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reactive.Linq;

namespace InterReact.SystemTests.Orders
{
    public class OrderServiceTests : TestCollectionBase
    {
        public OrderServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestPlaceOrder()
        {
            if (!Client.Request.Config.IsDemoAccount)
                throw new InvalidOperationException("Not demo account!");

            var contract = new Contract {
                SecurityType = SecurityType.Stock, Symbol = "TSLA", Currency = "USD", Exchange = "SMART" };

            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            var ticks = Client.Services.CreateTickObservable(contract).Publish();
            ticks.Connect();
            var priceTick = await ticks.OfTickType(x => x.PriceTick).Where(x => x.TickType == TickType.BidPrice).FirstAsync();

            Order order = new()
            {
                OrderAction = OrderAction.Buy,
                OrderType = OrderType.Limit,
                LimitPrice = priceTick.Price,
                TotalQuantity = 100,
                TimeInForce = TimeInForce.Day,
            };

            var orderId = Client.Request.GetNextId();

            Client.Request.PlaceOrder(orderId, order, contract);

            //Client.Request.CancelOrder(orderId);

            await Task.Delay(10000);
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
