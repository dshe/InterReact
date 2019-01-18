using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterReact;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Extensions;
/*
* If you cannot connect, be sure that Trader Workstation (TWS) is running on your machine
* and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
namespace ConsoleHelloWorld
{
    internal static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";
            Console.SetWindowSize(140, 30);
            Console.SetBufferSize(140, 30);

            // Create the InterReact client by connecting to TWS/Gateway.
            IInterReactClient client;
            try
            {
                client = await InterReactClient.Builder.BuildAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            // Print all messages as strings to the console and also observe any errors.
            client.Response.Stringify().Subscribe(
                onNext: Console.WriteLine, 
                onError: Console.WriteLine, 
                onCompleted: () => Console.WriteLine("Completed."));

            var contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART"
            };

            //client.Request.RequestCurrentTime();

            var contractDetailsList = await client.Services.ContractDetailsObservable(contract);

            var contractDetails = contractDetailsList.Single();

            Console.WriteLine($"Long Name: {contractDetails.LongName}.");

            await client.DisconnectAsync();

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
        }

    }
}

