using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Extensions;
/*
* Be sure that Trader Workstation (TWS) is running on your machine and that the following is set:
* File / GlobalConfiguration / API / Settings/ "Enable ActiveX and Socket Clients".
*/
#nullable enable

namespace HelloWorld
{
    internal static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";
            Console.SetWindowSize(140, 30);
            Console.SetBufferSize(140, 60);

            // Create the InterReact client by connecting to TWS/Gateway on your local machine.
            IInterReactClient client;
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

            var contractDataList = await client.Services.ContractDataObservable(contract);

            var contractData = contractDataList.Single();

            Console.WriteLine($"Long Name: {contractData.LongName}.");

            await client.DisconnectAsync();

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
        }

    }
}

