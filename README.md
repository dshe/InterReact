## InterReact&nbsp;&nbsp;[![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)

***Reactive C# API to Interactive Brokers***
- supports Interactive Brokers TWS/Gateway API 9.73
- supports NetStandard 2.0
- dependencies: Reactive Extensions, NodaTime.
- also requires the following solutions in same folder:  RxSockets, MinimalContainer, StringEnums, Stringification.
- demos provided for Console, Net Core, Windows Forms, WPF, and UWP.

**Notes**

TWS or Gateway must be running and API access mmust be enabled. To enable API access, navigate to Edit / Global Configuration / API / Settings and make sure the "Enable ActiveX and Socket Clients" option is checked.

**Example**

```csharp
// Create the InterReact client by connecting to TWS/Gateway using the default port and a random clientId.
IInterReactClient client = await InterReactClient.Builder.BuildAsync();

// Convert all messages received from IB to strings and then write them to the console.
client.Response.Stringify().Subscribe(
   onNext: Console.WriteLine, 
   onError: Console.WriteLine, 
   onCompleted: () => Console.WriteLine("Completed.")
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

// Get the one contract from the list.
ContractDetails contractDetails = contractDetailsList.Single();

// Print the LongName of the contract to the console.
Console.WriteLine($"Long Name: {contractDetails.LongName}.");

// Disconnect from IB.
await client.DisconnectAsync();
```
