using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Enums;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.SystemTests.Orders
{
    public class OrderRequestTests : BaseSystemTest
    {
        public OrderRequestTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task TestMarketPlaceOrder()
        {
            if (!Client.Config.IsDemoAccount)
                throw new Exception("Cannot place order. Not the demo account");

            var task = Client.Response
                .WithOrderId(Id)
                .OfType<Execution>()
                .FirstAsync().ToTask();

            var order = new Order
            {
                OrderId = Id,
                TradeAction = TradeAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Market
            };

            Client.Request.PlaceOrder(Id, order, Stock1);

            await task.Timeout();
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
                .FirstAsync().ToTask();

            Client.Request.RequestMarketData(Id, Stock1, null, marketDataOff: false, isSnapshot: true);

            var priceTick = await taskPrice.Timeout();

            Id = Client.Request.NextId();

            // place the order
            var taskOpenOrder = Client.Response
                .WithOrderId(Id)
                .OfType<OpenOrder>()
                .FirstAsync().ToTask();

            var order = new Order
            {
                OrderId = Id,
                TradeAction = TradeAction.Buy,
                TotalQuantity = 100,
                OrderType = OrderType.Limit,
                LimitPrice = priceTick.Price - 1
            };

            Client.Request.PlaceOrder(Id, order, Stock1);

            await taskOpenOrder.Timeout();

            // cancel the order
            var taskCancelled = Client.Response
                .WithOrderId(Id)
                .OfType<OrderStatusReport>()
                .Where(x => x.Status == OrderStatus.Cancelled)
                .FirstAsync().ToTask();

            Client.Request.CancelOrder(Id);

            await taskCancelled.Timeout();
        }

        [Fact]
        public async Task TestRequestOpenOrders()
        {
            var task = Client.Response
                .OfType<OpenOrderEnd>()
                .FirstAsync().ToTask();

            Client.Request.RequestOpenOrders();

            await task.Timeout();
        }

        [Fact]
        public async Task TestRequestExecutions()
        {
            var task = Client.Response
                .WithRequestId(Id)
                .OfType<ExecutionEnd>()
                .FirstAsync().ToTask();

            Client.Request.RequestExecutions(Id);

            await task.Timeout();
        }
    }
}

