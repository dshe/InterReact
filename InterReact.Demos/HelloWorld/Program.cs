using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InterReact;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Stringification;

/*
* Be sure that Trader Workstation (TWS) or gateway is running on your machine and the following is set:
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
                Console.SetBufferSize(140, 360);
            }

            // Log messages to the console.
            ILogger Logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger("Test");

            // Create the InterReact client by connecting to TWS/Gateway on your local machine.
            IInterReactClient client;
            try
            {
                // incoming message are sent to the Logger (which are sent to the console)
                client = await new InterReactClientBuilder(Logger)
                    .LogIncomingMessages() 
                    .BuildAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (!client.Request.Config.IsDemoAccount)
            {
                Console.WriteLine("Demo account is required sice an order is placed. Please login to the TWS demo account.");
                return;
            }

            Contract contract = new()
            {
                SecurityType = SecurityType.Stock,
                Symbol = "AMD",
                Currency = "USD",
                Exchange = "SMART"
            };

            client.Request.RequestMarketDataType(MarketDataType.Delayed);

            var ticks = client.Services.CreateTickObservable(contract).Publish().AutoConnect();

            PriceTick priceTick = await ticks
                .OfTickType(x => x.TickPrice)
                .Where(x => x.TickType == TickType.DelayedAskPrice)
                .FirstAsync();

            if (priceTick.Price <= 0)
            {
                Console.WriteLine($"Invalid price: {priceTick.Price}.");
                return;
            }

            Order order = new()
            {
                OrderAction = OrderAction.Buy,
                OrderType = OrderType.Limit,
                LimitPrice = priceTick.Price,
                TotalQuantity = 100,
            };

            int orderId = client.Request.GetNextId();
            client.Request.PlaceOrder(orderId, order, contract);

            // Press a key to exit.
            while (true)
            {
                if (Console.KeyAvailable)
                    break;
                await Task.Delay(200);
            }

            // Send a request to TWS/Gateway to for account update messages.
            //client.Request.RequestAccountUpdates(true);

            //Console.WriteLine(Environment.NewLine + "press a key to stop..." + Environment.NewLine);
            //Console.ReadKey();
            //await client.DisposeAsync();
            //Console.WriteLine(Environment.NewLine + "press a key to exit..." + Environment.NewLine);
            //Console.ReadKey();

            //subscription.Dispose();
        }
    }
}
