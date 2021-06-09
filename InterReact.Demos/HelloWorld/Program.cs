using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InterReact;


using Stringification;
/*
* Be sure that Trader Workstation (TWS) is running on your machine and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
namespace HelloWorld
{
    internal static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(140, 30);
                Console.SetBufferSize(140, 60);
            }

            // Create the InterReact client by connecting to TWS/Gateway on your local machine.
            IInterReactClient? client;
            try
            {
                client = await new InterReactClientBuilder().BuildAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            var contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART"
            };

            IObservable<Tick> tickObservable = client.Services.CreateTickObservable(contract);

            IDisposable subscription = tickObservable
                .OfType<TickPrice>()
                .Subscribe(onNext: tickPrice =>
                {
                    Console.WriteLine($"{Enum.GetName(tickPrice.TickType)} = {tickPrice.Price}");
                });

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
            Console.Clear();

            subscription.Dispose();
            await client.DisposeAsync();
        }
    }
}

