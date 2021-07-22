using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InterReact;
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

            // Print each message received from TWS/Gateway to the console.
            IDisposable subscription = client
                .Response
                .Subscribe(message =>
                {
                    Console.WriteLine(message.Stringify());
                });

            // Send a request to TWS/Gateway to for account update messages.
            client.Request.RequestAccountUpdates(true);

            await Task.Delay(3000);
            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
            Console.Clear();

            subscription.Dispose();
            await client.DisposeAsync();
        }
    }
}
