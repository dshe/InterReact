/*
* Be sure that Trader Workstation (TWS) or Gateway is running on your computer.
* Then ensure the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InterReact;
using Microsoft.Extensions.Logging;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    Console.BufferWidth = 2000;

// Create a logger which will log messages to the console.
ILogger Logger = LoggerFactory
    .Create(builder => builder
        .AddSimpleConsole(c => c.SingleLine = true)
        .SetMinimumLevel(LogLevel.Debug))
    .CreateLogger("HelloWorld");

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
IInterReactClient client = await new InterReactClientConnector()
    .WithLogger(Logger)
    .ConnectAsync();

if (!client.RemoteIPEndPoint.Port.IsIBDemoPort())
{
    Console.WriteLine("Demo account is required since an order will be placed. Please first login to the TWS demo account.");
    return;
}

Contract contract = new()
{
    /*
    SecurityType = SecurityType.Cash,
    Symbol = "EUR",
    Currency = "USD",
    Exchange = "IDEALPRO"
    */
    SecurityType = SecurityType.Stock,
    Symbol = "IBKR",
    Currency = "USD",
    Exchange = "SMART"
};

client.Request.RequestMarketDataType(MarketDataType.Delayed);

int requestId = client.Request.GetNextId();

client.Request.RequestMarketData(requestId, contract);

// Find the latest ask price for the security.
PriceTick askPriceTick = await client
    .Response
    .OfType<PriceTick>()
    .Where(t => t.RequestId == requestId)
    .Where(x => x.TickType == TickType.AskPrice || x.TickType == TickType.DelayedAskPrice)
    .Timeout(TimeSpan.FromSeconds(30)) // max time to wait for an ask price
    .FirstAsync();

if (askPriceTick.Price <= 0)
{
    Console.WriteLine($"\nInvalid price: {askPriceTick.Price}. Perhaps the market is closed.\n");
    return;
}

// Create the order.
Order order = new()
{
    OrderAction = OrderAction.Buy,
    OrderType = OrderType.Limit,
    LimitPrice = askPriceTick.Price,
    TotalQuantity = 100,
};

int orderId = client.Request.GetNextId();

// Start the task to receive the first OrderStatusReport, indicating either a filled or cancelled order, within the time limit.
Task<OrderStatusReport> orderTask = client
    .Response
    .OfType<OrderStatusReport>()
    .Where(x => x.OrderId == orderId)
    .Where(x => x.Status == OrderStatus.Filled || x.Status == OrderStatus.Cancelled || x.Status == OrderStatus.ApiCancelled)
    .FirstAsync()
    .Timeout(TimeSpan.FromSeconds(10)) // max time to fill the order
    .ToTask();

Console.WriteLine($"\nPlacing a buy order at limit price: {order.LimitPrice}.\n");
client.Request.PlaceOrder(orderId, order, contract);

try
{
    // Wait for order completion, or until timeout
    OrderStatusReport orderStatusReport = await orderTask;
    if (orderStatusReport.Status == OrderStatus.Filled)
        Console.WriteLine($"\nOrder was filled at price: {orderStatusReport.AverageFillPrice}.\n");
    else if (orderStatusReport.Status == OrderStatus.Cancelled || orderStatusReport.Status == OrderStatus.ApiCancelled)
        Console.WriteLine("\nOrder was cancelled.\n");
}
catch (TimeoutException)
{
    client.Request.CancelOrder(orderId);
    Console.WriteLine("\nTimeout! Order cancelled. Perhaps try again.\n");
}

client.Request.CancelMarketData(requestId);
await Task.Delay(1000);
await client.DisposeAsync();
