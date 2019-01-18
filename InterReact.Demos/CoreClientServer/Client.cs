using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.Messages;

namespace CoreClientServer
{
    internal class Client
    {
        private static void Write(string text) => Program.Write("Client: " + text, ConsoleColor.DarkMagenta);
        
        internal async Task Run(int port)
        {
            Write("Connecting.");

            var client = await InterReactClient.Builder.SetPort(port).BuildAsync();

            // write string messages to the console
            client.Response.OfType<string>().Subscribe(Write);

            Write("Sending messages...");
            Enumerable.Range(0, 100).ToList().ForEach(client.Request.CancelMarketData);

            // indicate test end
            client.Request.RequestMarketData(42, new Contract());

            // wait to get the first tickSize message, indicating test start
            await client.Response.OfType<TickSize>().FirstAsync();

            Write("Receiving...");

            // receive some messages to measure throughput
            var watch = new Stopwatch();
            watch.Start();
            var count = await client.Response.TakeWhile(m => m is TickSize).Count();
            watch.Stop();

            var frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
            Write($"Received {frequency:N0} messages/second.");

            Write("Disconnecting.");
            await client.DisconnectAsync();
            Write("Disconnected.");
        }
    }
}
