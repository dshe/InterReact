# InterReact&nbsp;&nbsp; [![License](https://img.shields.io/badge/Version-0.0.1-blue)]() [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- compatible with Interactive Brokers TWS/Gateway API 9.73
- supports **.NET 5.0**
- dependencies: RxSockets, StringEnums, Stringification, NodaTime, Reactive Extensions.
- demo applications: Console, NET Core, WPF, Windows Forms.

```csharp
interface IInterReactClient : IAsyncDisposable
{
    Request Request { get; }
    IObservable<object> Response { get; }
    Services Services { get; }
    Config Config { get; }
}
```
### Example ###
```csharp
// Create the InterReact client by connecting to TWS/Gateway using the default port and a random clientId.
IInterReactClient client = await new InterReactClientBuilder().BuildAsync();

// Convert all messages received from IB to strings and then write them to the console.
client.Response.Stringify().Subscribe(
   onNext:            Console.WriteLine, 
   onError:           Console.WriteLine, 
   onCompleted: () => Console.WriteLine("Completed.")
);

// Create a contract object.
Contract contract = new Contract
{
   SecurityType = SecurityType.Stock,
   Symbol       = "SPY",
   Currency     = "USD",
   Exchange     = "SMART"
};

// Make a request to IB for the contract details using the contract object.
IList<ContractData> contractDataList = await client.Services.ContractDataObservable(contract);

// Get the one contract from the list.
ContractData contractData = contractDataList.Single();

// Print the LongName of the contract to the console.
Console.WriteLine($"Long Name: {contractData.LongName}.");

// Disconnect from IB.
await client.DisposeAsync();
```
### Notes ###

TWS or Gateway must be running with API access enabled. In TWS, navigate to Edit / Global Configuration / API / Settings and ensure the option "Enable ActiveX and Socket Clients" is selected.
