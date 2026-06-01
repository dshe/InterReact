/*
* Be sure that Trader Workstation / Gateway (TWS) is running on your computer.
* Then, in TWS, ensure the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/

using System.Reactive.Linq;
using InterReact;

// Create the InterReact client by connecting to TWS/Gateway on your local machine.
//IInterReactClient client = await InterReactClient.ConnectAsync(o => o.TwsIpAddress = IPAddress.Parse("192.168.100.15"));
IInterReactClient client = await InterReactClient.CreateAsync();

// Create a contract object.
Contract contract = new()
{
    SecurityType = ContractSecurityType.Cash,
    Symbol = "EUR",
    Currency = "USD",
    Exchange = "IDEALPRO"
};

await client.Request.RequestMarketDataType(MarketDataType.Delayed);

// Display the alert messages which are not associated with a particular request.
client
    .Response
    .OfType<Alert>()
    .Where(msg => msg.RequestId == -1)
    .Subscribe(AlertMessage => Console.WriteLine(AlertMessage.Message));

// Create an observable and subscribe to observe ticks for the contract.
client
    .Service
    .CreateMarketDataObservable(contract)
    .OfTickClass(selector => selector.PriceTick)
    .Subscribe(priceTick => Console.WriteLine(priceTick.TickType + " = " + priceTick.Price));

Console.WriteLine(Environment.NewLine + "press a key to exit..." + Environment.NewLine);
await Console.In.ReadLineAsync();
Console.Clear();

// Disconnect from TWS/Gateway.
await client.DisposeAsync();
