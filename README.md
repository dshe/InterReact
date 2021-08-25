# InterReact&nbsp;&nbsp; [![License](https://img.shields.io/badge/Version-0.0.2-blue)]() [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- compatible with Interactive Brokers TWS/Gateway API 9.85.02 (Aug 05 2021).
- supports **.NET 5**.
- dependencies: RxSockets, StringEnums, Stringification, Reactive Extensions, NodaTime.
- demo applications: Console, WPF.

```csharp
interface IInterReact : IAsyncDisposable
{
    Request Request { get; }
    IObservable<object> Response { get; }
    Services Services { get; }
}
```
### Example ###
```csharp
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using InterReact;
```
```csharp
// Create the InterReact client by first connecting to TWS/Gateway on the local host.
IInterReact interReact = await InterReactBuilder.Create().BuildAsync();

// Create a contract object.
Contract contract = new Contract
{
   SecurityType = SecurityType.Stock,
   Symbol       = "SPY",
   Currency     = "USD",
   Exchange     = "SMART"
};

// Create an observable which can observe ticks for the contract.
IObservable<ITick> tickObservable = interReact.Services.CreateTickObservable(contract);

// Subscribe to the observable to start observing ticks.
IDisposable subscription = tickObservable
    .OfType(selector => selector.TickPrice)
    .Subscribe(onNext: tickPrice =>
    {
        // Write price ticks to the console.
        Console.WriteLine($"{Enum.GetName(tickPrice.TickType)} = {tickPrice.Price}");
    });

Console.WriteLine(Environment.NewLine + "press a key to exit...");
Console.ReadKey();
Console.Clear();

// Dispose the subscription to stop observing ticks.
subscription.Dispose();

// Disconnect from TWS/Gateway.
interReact.DisposeAsync();
```
### Notes ###

TWS or Gateway must be running with API access enabled. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option "Enable ActiveX and Socket Clients" is selected.
