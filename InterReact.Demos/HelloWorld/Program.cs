using System;
using System.Linq;
using System.Reactive.Linq;
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

            // Print all messages as strings to the console.
            client.Response.StringifyItems().Subscribe(
                onNext: Console.WriteLine,
                //onError: Console.WriteLine,
                onCompleted: () => Console.WriteLine("Completed."));

            var contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = "C",
                Currency = "USD",
                Exchange = "NYSE"
            };

            var contractDataList = await client.Services.CreateContractDataObservable(contract);

            var contractData = contractDataList.Single();

            Console.WriteLine($"Long Name: {contractData.LongName}.");

            // allow some time to display data
            await Task.Delay(3000);
            await client.DisposeAsync();

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
        }
    }
}

