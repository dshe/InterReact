using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using Xunit;
//using Stringification;
/*
* Be sure that Trader Workstation (TWS) is running on your machine and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
namespace InterReactSamples
{
    [Trait("Category", "Examples")]
    public class SimpleExample
    {
        [Fact]
        public async Task Test()
        {
            // Create the InterReact client by connecting to TWS/Gateway on your local machine.
            IInterReactClient interReact = await InterReactClientBuilder.Create().BuildAsync();

            var contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART"
            };

            IObservable<Union<Tick, Alert>> observable = interReact
                .Services
                .CreateTickObservable(contract);

            IDisposable subscription = observable
                .OfTickType(tickType => tickType.PriceTick)
                .Subscribe(onNext: tickPrice =>
                {
                    Console.WriteLine($"{Enum.GetName(tickPrice.TickType)} = {tickPrice.Price}");
                });

            //Console.WriteLine(Environment.NewLine + "press a key to exit...");
            //Console.ReadKey();
            //Console.Clear();

            subscription.Dispose();
            await interReact.DisposeAsync();
        }
    }
}

