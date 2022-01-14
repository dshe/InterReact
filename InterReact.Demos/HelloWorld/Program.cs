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
    .Create(builder => builder.AddSimpleConsole(c => c.SingleLine = true))
    .CreateLogger("Hello");

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
// WithLogger(Logger, false) sends InterReact debug messages to the Logger, which will be sent to the console.
// WithLogger(Logger, true) will also send all incoming messages to the Logger. Try it.
IInterReactClient client = await InterReactClientBuilder
    .Create()
    .WithLogger(Logger)
    .BuildAsync();

if (!client.Request.Builder.IsDemoAccount)
{
    Console.WriteLine("Demo account is required since an order is placed. Please login to the TWS demo account.");
    return;
}

Contract contract = new()
{
    SecurityType = SecurityType.Cash,
    Symbol = "EUR",
    Currency = "USD",
    Exchange = "IDEALPRO"
};

int tickRequestId = client.Request.GetNextId();
client.Request.RequestMarketData(tickRequestId, contract);

// Find the latest ask price for the security.
PriceTick priceTick = await client
    .Response
    .OfType<PriceTick>()
    .Where(t => t.RequestId == tickRequestId)
    .Where(x => x.TickType == TickType.AskPrice)
    .FirstAsync();

if (priceTick.Price <= 0)
{
    Console.WriteLine($"\nInvalid price: {priceTick.Price}. Perhaps the market is closed.\n");
    return;
}

// Create the order.
Order order = new()
{
    OrderAction = OrderAction.Buy,
    OrderType = OrderType.Limit,
    LimitPrice = Math.Round(priceTick.Price + .0001, 4),
    TotalQuantity = 50000,
};

int orderId = client.Request.GetNextId();

// Start the task to receive the first OrderStatusReport, indicating either a filled or cancelled order, within the time limit.
Task<OrderStatusReport> orderTask = client
    .Response
    .OfType<OrderStatusReport>()
    .Where(x => x.OrderId == orderId)
    .Where(x => x.Status == OrderStatus.Filled || x.Status == OrderStatus.Cancelled || x.Status == OrderStatus.ApiCancelled)
    .FirstAsync()
    .Timeout(TimeSpan.FromSeconds(10))
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

client.Request.CancelMarketData(tickRequestId);
await Task.Delay(1000);
await client.DisposeAsync();
