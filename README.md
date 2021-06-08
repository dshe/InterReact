# InterReact&nbsp;&nbsp; [![License](https://img.shields.io/badge/Version-0.0.1-blue)]() [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- compatible with Interactive Brokers TWS/Gateway API 9.73
- supports **.NET 5.0**
- dependencies: RxSockets, StringEnums, Stringification, NodaTime, Reactive Extensions.
- demo applications: Console, NET Core, WPF, Windows Forms.

```csharp
interface IInterReactClient : IAsyncDisposable
{
    Config Config { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Services Services { get; }
}
```
### Example ###
```csharp
// Create the InterReact client by connecting to TWS/Gateway using the default port and a random clientId.
IInterReactClient client = await new InterReactClientBuilder().BuildAsync();

// Create a contract object.
Contract contract = new Contract
{
   SecurityType = SecurityType.Stock,
   Symbol       = "SPY",
   Currency     = "USD",
   Exchange     = "SMART"
};

// Create an observbale which will return ticks for the contract.
IObservable<Tick> ticks = client.Services.CreateTickObservable(contract);

// Subscribe to the observable to start receiving ticks.
ticks.OfType<TickPrice>().Subscribe(tickPrice =>
{
    Console.WriteLine($"Price = {tickPrice.Price}");
});

Console.WriteLine(Environment.NewLine + "press a key to exit...");
Console.ReadKey();
Console.Clear();

// Disconnect from TWS/Gateway.
await client.DisposeAsync();
```
### Notes ###

TWS or Gateway must be running with API access enabled. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option "Enable ActiveX and Socket Clients" is selected.
