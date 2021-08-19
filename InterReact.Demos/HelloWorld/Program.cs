/*
* Be sure that Trader Workstation (TWS) or Gateway is running on your computer.
* Then ensure the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact;
using Microsoft.Extensions.Logging;

Console.Title = "InterReact";

// Create a logger which will log messages to the console.
ILogger Logger = LoggerFactory
    .Create(builder => builder.AddSimpleConsole(c => c.SingleLine = true))
    .CreateLogger("Hello");

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
// WithLogger(Logger, false) sends InterReact debug messages to the Logger (which are then sent to the console).
// WithLogger(Logger, true) will also send all incoming message to the Logger. Try it.
IInterReactClient client = await InterReactClientBuilder
    .Create()
    .WithLogger(Logger, false)
    .BuildAsync();

if (!client.Request.Config.IsDemoAccount)
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

// Create an observable to receive contract ticks.
IConnectableObservable<Union<Tick, Alert>> tickObservable = client
    .Services
    .CreateTickObservable(contract)
    .Publish();

// Start the tick observable.
IDisposable tickConnection = tickObservable.Connect();

// Find the latest ask price for the security.
PriceTick priceTick = await tickObservable
    .OfTickType(x => x.PriceTick)
    .Where(x => x.TickType == TickType.AskPrice)
    .FirstAsync();

if (priceTick.Price <= 0)
{
    Console.WriteLine($"Invalid price: {priceTick.Price}.");
    return;
}

// Set the price so that it will probably execute.
double orderPrice = Math.Round(priceTick.Price + .0001,4);

// Create the order.
Order order = new()
{
    OrderAction = OrderAction.Buy,
    OrderType = OrderType.Limit,
    LimitPrice = orderPrice,
    TotalQuantity = 50000,
};

int orderId = client.Request.GetNextId();

// Write order alerts, if any, to the console.
IDisposable alertSubscription = client
    .Response
    .OfType<Alert>()
    .Where(a => a.OrderId == orderId)
    .Subscribe(x => Console.WriteLine(x.Message));

// Start the task to write the Execution report, if any, to the console, within the time limit.
Task<Execution> excecutionTask = client
    .Response
    .OfType<Execution>()
    .Where(x => x.OrderId == orderId)
    .FirstAsync()
    .Timeout(TimeSpan.FromSeconds(10))
    .ToTask();

Console.WriteLine($"Placing a buy order at limit price: {orderPrice}.");
client.Request.PlaceOrder(orderId, order, contract);

try
{
    // wait for execution, or until timeout (observable)
    Execution execution = await excecutionTask;
    Console.WriteLine($"Executed at price: {execution.Price}.");
}
catch (TimeoutException)
{
    Console.WriteLine("Order timeout. Cancelled. (perhaps try again)");
    client.Request.CancelOrder(orderId);
}

tickConnection.Dispose();
alertSubscription.Dispose();
await client.DisposeAsync();
