using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact;
using Microsoft.Extensions.Logging;

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

            // Create a logger to log messages to the console.
            ILogger Logger = LoggerFactory
                .Create(builder => builder.AddSimpleConsole(c => c.SingleLine = true))
                .CreateLogger("Hello");

            // Create the InterReact client by connecting to TWS/Gateway on your local machine.
            // WithLogger(Logger, true) will send incoming message to the Logger (which are then sent to the console).
            IInterReactClient client = await InterReactClientBuilder.Create()
                .WithLogger(Logger, false)
                .BuildAsync();

            if (!client.Request.Config.IsDemoAccount)
            {
                Console.WriteLine("Demo account is required since an order is placed. Please login to the TWS demo account.");
                return;
            }

            Contract contract = new()
            {
                SecurityType = SecurityType.Stock,
                Symbol = "AMD",
                Currency = "USD",
                Exchange = "SMART"
            };

            // Use delayed data since we don't have a subscription.
            client.Request.RequestMarketDataType(MarketDataType.Delayed);

            IConnectableObservable<Union<Tick, Alert>> tickObservable = client
                .Services
                .CreateTickObservable(contract)
                .Publish();

            var connection = tickObservable.Connect();

            // Find the latest ask price for the security.
            PriceTick priceTick = await tickObservable
                .OfTickType(x => x.PriceTick)
                .Where(x => x.TickType == TickType.DelayedAskPrice)
                .FirstAsync();

            if (priceTick.Price <= 0)
            {
                Console.WriteLine($"Invalid price: {priceTick.Price}.");
                return;
            }

            var orderPrice = priceTick.Price + .30;

            Order order = new()
            {
                OrderAction = OrderAction.Buy,
                OrderType = OrderType.Limit,
                LimitPrice = orderPrice,
                TotalQuantity = 100,
            };

            int orderId = client.Request.GetNextId();

            var alertSubscription = client
                .Response
                .OfType<Alert>()
                .Where(a => a.OrderId == orderId)
                .Subscribe(x => Console.WriteLine(x.Message));

            Task<Execution> excecutionTask = client
                .Response
                .OfType<Execution>()
                .Where(x => x.OrderId == orderId)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(10))
                .ToTask();

            Console.WriteLine($"Placing a buy order at limit price: {orderPrice}.");
            client.Request.PlaceOrder(orderId, order, contract);

            try
            {
                // wait for execution, until observable timeout
                var execution = await excecutionTask;
                Console.WriteLine($"Executed at price: {execution.Price}.");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Order timeout. Cancelled. (perhaps try again)");
                client.Request.CancelOrder(orderId);
            }

            connection.Dispose();
            alertSubscription.Dispose();
            await client.DisposeAsync();
        }
    }
}
