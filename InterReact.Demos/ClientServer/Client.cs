using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;

using Microsoft.Extensions.Logging;

namespace CoreClientServer
{
    internal static class Client
    {
        internal static async Task Run(int port, ILogger logger, ILogger libLogger)
        {
            var client = await new InterReactBuilder(libLogger)
                .SetPort(port)
                .BuildAsync()
                .ConfigureAwait(false);

            logger.LogInformation("Connected to server.");
            client.Response.OfType<string>().Subscribe(x => logger.LogDebug(x));

            logger.LogInformation("Sending messages...");
            Enumerable.Range(0, 100).ToList().ForEach(client.Request.CancelMarketData);

            // indicate test end
            client.Request.RequestMarketData(42, new Contract());

            // wait to get the first tickSize message, indicating test start
            await client.Response.OfType<TickSize>().FirstAsync();

            logger.LogInformation("Receiving...");

            // receive some messages to measure throughput
            var watch = new Stopwatch();
            watch.Start();
            var count = await client.Response.TakeWhile(m => m is TickSize).Count();
            watch.Stop();

            var frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
            logger.LogInformation($"Received {frequency:N0} messages/second.");

            logger.LogInformation("Disconnecting.");
            await client.DisposeAsync();
            logger.LogInformation("Disconnected.");
        }
    }
}
