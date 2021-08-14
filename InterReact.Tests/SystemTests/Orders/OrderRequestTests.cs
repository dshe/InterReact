using InterReact;
using InterReact.SystemTests;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Orders
{
    public class OrderRequestTests : TestCollectionBase
    {
        public OrderRequestTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestMarketPlaceOrder()
        {
            if (!Client.Request.Config.IsDemoAccount)
                throw new Exception("Cannot place order. Not the demo account");

            var id = Client.Request.GetNextId();

            var task = Client.Response
                .OfType<Execution>()
                .Where(x => x.RequestId == id)
                .FirstAsync().Timeout(TimeSpan.FromSeconds(3)).ToTask();

            var order = new Order
            {
                OrderId = id,
                OrderAction = OrderAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Market
            };

            Client.Request.PlaceOrder(id, order, Stock1);

            await task;
        }

        [Fact]
        public async Task TestPlaceLimitOrder()
        {
            if (!Client.Request.Config.IsDemoAccount)
                throw new Exception("Cannot place order. Not the demo account");

            var id = Client.Request.GetNextId();

            // find the price
            var taskPrice = Client.Response
                .OfType<PriceTick>()
                .Where(x => x.RequestId == id)
                .Where(x => x.TickType == TickType.AskPrice)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.RequestMarketData(id, Stock1, null, isSnapshot: true);

            var priceTick = await taskPrice;

            // place the order
            var taskOpenOrder = Client.Response
                .OfType<OpenOrder>()
                .Where(x => x.OrderId == id)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            var order = new Order
            {
                OrderId = id,
                OrderAction = OrderAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Limit,
                LimitPrice = priceTick.Price - 1
            };

            Client.Request.PlaceOrder(id, order, Stock1);

            await taskOpenOrder;

            // cancel the order
            var taskCancelled = Client.Response
                .OfType<OrderStatusReport>()
                .Where(x => x.OrderId == id)
                .Where(x => x.Status == OrderStatus.Cancelled)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.CancelOrder(id);

            await taskCancelled;
        }

        [Fact]
        public async Task TestRequestOpenOrders()
        {
            var task = Client.Response
                .OfType<OpenOrderEnd>()
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.RequestOpenOrders();

            await task;
        }

        [Fact]
        public async Task TestRequestExecutions()
        {
            var id = Client.Request.GetNextId();

            var task = Client.Response
                .OfType<ExecutionEnd>()
                .Where(x => x.RequestId == id)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.RequestExecutions(id);

            await task;
        }
    }
}

