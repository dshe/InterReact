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

// Create a logger which will write messages to the console.
ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
    .AddSimpleConsole(c => c.SingleLine = true)
    .SetMinimumLevel(LogLevel.Information));

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
IInterReactClient client = await InterReactClient.ConnectAsync(options => options.LogFactory = loggerFactory);

if (!client.RemoteIpEndPoint.IsUsingIBDemoPort())
{
    Console.WriteLine("Demo account is required since an order will be placed. Please first login to the TWS demo account.");
    return;
}

Contract contract = new()
{
    SecurityType = ContractSecurityType.Cash,
    Symbol = "EUR",
    Currency = "USD",
    Exchange = "IDEALPRO"
};

// Find the latest ask price for the security.
PriceTick askPriceTick = await client
    .Service.CreateMarketDataSnapshotObservable(contract)
    .OfTickClass(c => c.PriceTick)
    .Where(priceTick => priceTick.TickType is TickType.AskPrice)
    .FirstAsync();

if (askPriceTick.Price > 0)
    Console.WriteLine($"\nFound the latest ask price: {askPriceTick.Price}.\n");
else
{
    Console.WriteLine($"\nInvalid price: {askPriceTick.Price}. Perhaps the market is closed.\n");
    return;
}

// Create the order.
Order order = new()
{
    Action = OrderAction.Buy,
    OrderType = OrderTypes.Limit,
    LimitPrice = askPriceTick.Price,
    TotalQuantity = 50000,
};

Console.WriteLine($"Placing a buy order at price: {order.LimitPrice}.\n");

OrderMonitor orderMonitor = client.Service.PlaceOrder(order, contract);

// Display all the types of messages received for the order.
orderMonitor.Messages.OfType<AlertMessage>().Subscribe(AlertMessage => Console.WriteLine(AlertMessage.Message));

try
{
    // Wait for an OrderStatusReport, indicating either a filled or cancelled order, within the time limit.
    OrderStatusReport osr = await orderMonitor
        .Messages
        .OfType<OrderStatusReport>()
        .Where(x => x.Status == OrderStatus.Filled || x.Status == OrderStatus.Cancelled || x.Status == OrderStatus.ApiCancelled)
        .Timeout(TimeSpan.FromSeconds(5)) // max time to fill the order
        .FirstAsync();

    // Wait for order completion, or until timeout
    //OrderStatusReport orderStatusReport = await orderTask;
    if (osr.Status == OrderStatus.Filled)
        Console.WriteLine($"\nOrder was filled at price: {osr.AverageFillPrice}.\n");
    else if (osr.Status == OrderStatus.Cancelled || osr.Status == OrderStatus.ApiCancelled)
        Console.WriteLine("\nOrder was cancelled.\n");
}
catch (TimeoutException)
{
    orderMonitor.CancelOrder();
    Console.WriteLine("\nTimeout! Order cancelled. Perhaps try again.\n");
}

Console.WriteLine(Environment.NewLine + "press a key to exit..." + Environment.NewLine);
Console.ReadKey();

await client.DisposeAsync();
