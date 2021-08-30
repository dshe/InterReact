using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using Xunit;
/*
* Be sure that Trader Workstation (TWS) is running on your machine and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
namespace InterReact.SystemTests.Other
{
    [Trait("Category", "Examples")]
    public class SimplestExample
    {
        [Fact]
        public async Task Test()
        {
            // Create the InterReact client by first connecting to TWS/Gateway on the local host.
            IInterReactClient interReact = await InterReactClientBuilder
                .Create()
                .BuildAsync();

            // Create a contract object.
            Contract contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART"
            };

            // Create and then subscribe to the observable which can observe ticks for the contract.
            IDisposable subscription = interReact
                .Services
                .CreateTickObservable(contract)
                .OfITickClass(tickType => tickType.PriceTick)
                .Subscribe(onNext: tickPrice => Console.WriteLine($"Price = {tickPrice.Price}"));

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
            Console.Clear();

            // Dispose the subscription to stop receiving ticks.
            subscription.Dispose();

            // Disconnect from TWS/Gateway.
            await interReact.DisposeAsync();
        }
    }
}

