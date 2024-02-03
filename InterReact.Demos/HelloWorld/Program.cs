/*
* Be sure that Trader Workstation / Gateway (TWS) is running on your computer.
* Then, in TWS, ensure the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/

using InterReact;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

// Create a logger which will write messages to the console.
ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
    .AddSimpleConsole(c => c.SingleLine = true)
    .SetMinimumLevel(LogLevel.Debug));

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
IInterReactClient client = await InterReactClient.ConnectAsync(options => options.LogFactory = loggerFactory);

if (!client.RemoteIpEndPoint.Port.IsIBDemoPort())
{
    Console.WriteLine("Demo account is required since an order will be placed. Please first login to the TWS demo account.");
    return;
}

Contract contract = new()
{
    SecurityType = ContractSecurityType.Stock,
    Symbol = "IBKR",
    Currency = "USD",
    Exchange = "SMART"
};

client.Request.RequestMarketDataType(MarketDataType.Delayed);

int id = client.Request.GetNextId();

client.Request.RequestMarketData(id, contract);

// Find the latest ask price for the security.
PriceTick askPriceTick = await client
    .Response
    .OfType<PriceTick>()
    .Where(t => t.RequestId == id)
    .Where(x => x.TickType is TickType.AskPrice or TickType.DelayedAskPrice)
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
    Action = OrderAction.Buy,
    OrderType = OrderTypes.Limit,
    LimitPrice = askPriceTick.Price + .50,
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

client.Request.CancelMarketData(id);
await Task.Delay(2000);
await client.DisposeAsync();
