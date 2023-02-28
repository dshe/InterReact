# InterReact&nbsp;&nbsp; [![Version](https://img.shields.io/badge/Version-0.3.0-blue)](../..) [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0) [![Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

***Reactive C# API to Interactive Brokers Trader Workstation***
- **.NET 6.0** library
- compatible with Interactive Brokers API 10.19 (Nov 2022)
- dependencies: RxSockets, StringEnums, NodaTime
- demo applications: Console, WPF

```csharp
interface IInterReactClient : IAsyncDisposable
{
    IPEndPoint RemoteIPEndPoint { get; }
    public ServerVersion ServerVersion { get; }
    int ClientId { get; }
    Request Request { get; }
    IObservable<object> Response { get; }
    Service Service { get; }
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
// Create the InterReact client by connecting to TWS/Gateway on the local host.
IInterReactClient client = await new InterReactClientConnector().ConnectAsync();

// Create a contract object.
Contract contract = new()
{
   SecurityType = SecurityType.Stock,
   Symbol       = "SPY",
   Currency     = "USD",
   Exchange     = "SMART"
};

// Create and then subscribe to the observable which can observe ticks for the contract.
IDisposable subscription = client
    .Service
    .CreateTickObservable(contract)
    .OfTickClass(selector => selector.PriceTick)
    .Where(tick => tick.TickType == TickType.LastPrice)
    .Subscribe(onNext: priceTick => Console.WriteLine($"Last Price = {priceTick.Price}"));

Console.WriteLine(Environment.NewLine + "press a key to exit...");
Console.ReadKey();
Console.Clear();

// Dispose the subscription to stop receiving ticks.
subscription.Dispose();

// Disconnect from TWS/Gateway.
await client.DisposeAsync();
```
### Notes ###

Interactive Brokers Trader Workstation (TWS) or Gateway must be running with API access enabled. In TWS, navigate to Edit / Global Configuration / API / Settings. Ensure the option "Enable ActiveX and Socket Clients" is selected.
