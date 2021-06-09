# InterReact&nbsp;&nbsp; [![License](https://img.shields.io/badge/Version-0.0.1-blue)]() [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- compatible with Interactive Brokers TWS/Gateway API 9.73
- supports **.NET 5.0**
- dependencies: RxSockets, StringEnums, Stringification, NodaTime, Reactive Extensions.
- demo applications: Console, WPF.

```csharp
interface IInterReact : IAsyncDisposable
{
    Config Config { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Services Services { get; }
}
```
### Example ###
```csharp
// Create the InterReact client by first connecting to TWS/Gateway on the local host.
IInterReact interreact = await new InterReactBuilder().BuildAsync();

// Create a contract object.
Contract contract = new Contract
{
   SecurityType = SecurityType.Stock,
   Symbol       = "SPY",
   Currency     = "USD",
   Exchange     = "SMART"
};

// Create an observable which will receive ticks for the contract.
IObservable<Tick> tickObservable = interreact.Services.CreateTickObservable(contract);

// Subscribe to the observable to start receiving ticks.
IDisposable subscription = tickObservable
    .OfType<TickPrice>()
    .Subscribe(onNext: tickPrice =>
    {
        // Write ticks to the console.
        Console.WriteLine($"{Enum.GetName(tickPrice.TickType)} = {tickPrice.Price}");
    });

Console.WriteLine(Environment.NewLine + "press a key to exit...");
Console.ReadKey();
Console.Clear();

// Dispose the subscription to stop receiving ticks.
subscription.Dispose();

// Disconnect from TWS/Gateway.
interreact client.DisposeAsync();
```
### Notes ###

TWS or Gateway must be running with API access enabled. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option "Enable ActiveX and Socket Clients" is selected.
