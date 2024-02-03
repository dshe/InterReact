using System.Reactive.Linq;
/*
* Be sure that Trader Workstation (TWS) is running on your machine and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
namespace Example;

public class SimplestExample
{
    [Fact]
    public async Task Test()
    {
        // Create the InterReact client by first connecting to TWS/Gateway on the local host.
        IInterReactClient interReact = await InterReactClient.ConnectAsync();

        // Create a contract object.
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "SPY",
            Currency = "USD",
            Exchange = "SMART"
        };

        // Create and then subscribe to the observable which can observe ticks for the contract.
        IDisposable subscription = interReact
            .Service
            .CreateTickObservable(contract)
            .OfTickClass(selector => selector.PriceTick)
            .Where(tick => tick.TickType == TickType.LastPrice)
            .Subscribe(onNext: priceTick => Console.WriteLine($"Last Price = {priceTick.Price}"));

        //Console.WriteLine(Environment.NewLine + "press a key to exit...");
        //Console.ReadKey();
        //Console.Clear();

        // Dispose the subscription to stop receiving ticks.
        subscription.Dispose();

        // Disconnect from TWS/Gateway.
        await interReact.DisposeAsync();
    }
}

