using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact;
using InterReact.Extensions;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;

namespace SystemTests.Orders
{
    public class OrderRequestTests : BaseTest
    {
        public OrderRequestTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestMarketPlaceOrder()
        {
            if (!Client.Config.IsDemoAccount)
                throw new Exception("Cannot place order. Not the demo account");

            var task = Client.Response
                .WithOrderId(Id)
                .OfType<Execution>()
                .FirstAsync().Timeout(TimeSpan.FromSeconds(3)).ToTask();

            var order = new Order
            {
                OrderId = Id,
                TradeAction = TradeAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Market
            };

            Client.Request.PlaceOrder(Id, order, Stock1);

            await task;
        }

        [Fact]
        public async Task TestPlaceLimitOrder()
        {
            if (!Client.Config.IsDemoAccount)
                throw new Exception("Cannot place order. Not the demo account");

            // find the price
            var taskPrice = Client.Response
                .WithRequestId(Id)
                .OfType<TickPrice>()
                .Where(x => x.TickType == TickType.AskPrice)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.RequestMarketData(Id, Stock1, null, marketDataOff: false, isSnapshot: true);

            var priceTick = await taskPrice;

            Id = Client.Request.GetNextId();

            // place the order
            var taskOpenOrder = Client.Response
                .WithOrderId(Id)
                .OfType<OpenOrder>()
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            var order = new Order
            {
                OrderId = Id,
                TradeAction = TradeAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Limit,
                LimitPrice = priceTick.Price - 1
            };

            Client.Request.PlaceOrder(Id, order, Stock1);

            await taskOpenOrder;

            // cancel the order
            var taskCancelled = Client.Response
                .WithOrderId(Id)
                .OfType<OrderStatusReport>()
                .Where(x => x.Status == OrderStatus.Cancelled)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.CancelOrder(Id);

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
            var task = Client.Response
                .WithRequestId(Id)
                .OfType<ExecutionEnd>()
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(3))
                .ToTask();

            Client.Request.RequestExecutions(Id);

            await task;
        }
    }
}

