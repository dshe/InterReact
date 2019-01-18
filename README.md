## InterReact&nbsp;&nbsp;[![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- supports IB API 9.73
- supports NetStandard 2.0
- dependencies: Reactive Extensions, NodaTime
- also requires the following solutions in same folder:  RxSockets, MinimalContainer, StringEnums, Stringification.
- demos provided for Console, Core, Forms, WPF, and UWP.

**Example**

```csharp
// Create the InterReact client by connecting to TWS/Gateway using the default port and a random clientId
IInterReactClient client = await InterReactClient.Builder.BuildAsync();

// Print all messages as strings to the console.
client.Response.Stringify().Subscribe(
   onNext: Console.WriteLine, 
   onError: Console.WriteLine, 
   onCompleted: () => Console.WriteLine("Completed.")\
);

// Create a contract object.
Contract contract = new Contract
{
   SecurityType = SecurityType.Stock,
   Symbol = "SPY",
   Currency = "USD",
   Exchange = "SMART"
};

// Make a request to IB for the contract details using the contract object.
IList<ContractDetails> contractDetailsList = await client.Services.ContractDetailsObservable(contract);

// Take the first contract in the list.
ContractDetails contractDetails = contractDetailsList.Single();

// Print the LongName of the contract to the console.
Console.WriteLine($"Long Name: {contractDetails.LongName}.");

// Disconnect the API from IB.
await client.DisconnectAsync();
```
