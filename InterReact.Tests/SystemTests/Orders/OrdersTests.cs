using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Orders;

public class OrdersTests : TestCollectionBase
{
    public OrdersTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TestMarketPlaceOrder()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        var orderId = Client.Request.GetNextOrderId();

        var task = Client.Response
            .OfType<Execution>()
            .Where(x => x.OrderId == orderId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        var order = new Order
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Market
        };

        Client.Request.PlaceOrder(orderId, order, StockContract1);

        await task;
    }

    [Fact]
    public async Task TestPlaceLimitOrder()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        int orderId = Client.Request.GetNextOrderId();
        int requestId = Client.Request.GetNextRequestId();

        // find the price
        var taskPrice = Client.Response
            .OfType<PriceTick>()
            .Where(x => x.RequestId == requestId)
            .Where(x => x.TickType == TickType.AskPrice)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(10))
            .ToTask();

        Client.Request.RequestMarketData(requestId, StockContract1, null, isSnapshot: true);

        var priceTick = await taskPrice;

        // place the order
        var taskOpenOrder = Client.Response
            .OfType<OpenOrder>()
            .Where(x => x.OrderId == orderId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        var order = new Order
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Limit,
            LimitPrice = priceTick.Price - 1 // below market
        };

        Client.Request.PlaceOrder(orderId, order, StockContract1);

        await taskOpenOrder;

        // cancel the order
        var taskCancelled = Client.Response
            .OfType<OrderStatusReport>()
            .Where(x => x.OrderId == orderId)
            .Where(x => x.Status == OrderStatus.Cancelled)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        Client.Request.CancelOrder(orderId);

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
        var requestId = Client.Request.GetNextRequestId();

        var task = Client.Response
            .OfType<ExecutionEnd>()
            .Where(x => x.RequestId == requestId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(3))
            .ToTask();

        Client.Request.RequestExecutions(requestId);

        await task;
    }

    [Fact]
    public async Task TestOrderStatus()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        var requestId = Client.Request.GetNextRequestId();
        var orderId = Client.Request.GetNextOrderId();

        long statusReceived = 0;
        //IObservable<object> orderStatusService = Client.Service.CreateOpenOrdersObservable();
        //var orderStatusObserver = orderStatusService.Subscribe(async x =>
        var orderStatusObserver = Client.Service.OrderStatusReportObservable.Subscribe(async x =>
        {
            if (!(x is InterReact.OrderStatusReport))
                return;

            OrderStatusReport? status = x as InterReact.OrderStatusReport;

            if (status != null)
            {
                Interlocked.Increment(ref statusReceived);
                Logger.LogInformation("Status Received: ", status.Status);
            }
        });


        var taskOpenOrder = Client.Response
            .OfType<OpenOrder>()
            .Where(x => x.OrderId == orderId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();


        var order = new Order
        {
            OrderId = orderId,
            OrderAction = OrderAction.Buy,
            TotalQuantity = 100,
            OrderType = OrderType.Limit,
            LimitPrice = 1 // below market
        };

        Client.Request.PlaceOrder(orderId, order, StockContract1);

        await taskOpenOrder;
        Client.Request.RequestAllOpenOrders();
        
        
       // cancel the order
        var taskCancelled = Client.Response
            .OfType<OrderStatusReport>()
            .Where(x => x.OrderId == orderId)
            .Where(x => x.Status == OrderStatus.Cancelled)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        Client.Request.CancelOrder(orderId);

        await taskCancelled;
       
        Client.Request.RequestAllOpenOrders();
        Thread.Sleep(4000);

        long lastValue = Interlocked.Read(ref statusReceived);
        if (lastValue == 0) 
        {
            throw new Exception("No order status received");
        }
        else
        {
            Logger.LogInformation("Number Status Reports Received: ", lastValue);
        }
    }



}

