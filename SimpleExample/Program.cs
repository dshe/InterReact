/*
* Be sure that Trader Workstation / Gateway (TWS) is running on your computer.
* Then, in TWS, ensure the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/

using System.Reactive.Linq;
using InterReact;

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
IInterReactClient client = await InterReactClient.ConnectAsync();

// Create a contract object.
Contract contract = new()
{
    SecurityType = ContractSecurityType.Stock,
    Symbol = "AMZN",
    Currency = "USD",
    Exchange = "SMART"
};

// Create and then subscribe to the observable which can observe ticks for the contract.
IDisposable subscription = client
    .Service
    .CreateTickObservable(contract)
    .OfTickClass(selector => selector.PriceTick)
    .Where(tick => tick.TickType == TickType.LastPrice)
    .Subscribe(onNext: priceTick => Console.WriteLine($"Price = {priceTick.Price}"));

Console.WriteLine(Environment.NewLine + "press a key to exit...");
Console.ReadKey();
Console.Clear();

// Dispose the subscription to stop receiving ticks.
subscription.Dispose();

// Disconnect from TWS/Gateway.
await client.DisposeAsync();
