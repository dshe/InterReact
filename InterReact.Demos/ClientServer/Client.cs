using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.Messages;
using Microsoft.Extensions.Logging;

#nullable enable

namespace CoreClientServer
{
    internal class Client
    {
        private readonly int Port;
        private readonly ILogger Logger;
        private readonly MyLoggerFactory LoggerFactory;

        internal Client(int port, ILogger logger, MyLoggerFactory loggerFactory)
        {
            Port = port;
            Logger = logger;
            LoggerFactory = loggerFactory;
        }

        internal async Task Run()
        {
            // hangs on next line
            var client = await new InterReactClientBuilder(LoggerFactory).SetPort(Port).BuildAsync().ConfigureAwait(false);
            
            Logger.LogInformation("Connected to server.");
            client.Response.OfType<string>().Subscribe(x => Logger.LogDebug(x));

            Logger.LogInformation("Sending messages...");
            Enumerable.Range(0, 100).ToList().ForEach(client.Request.CancelMarketData);

            // indicate test end
            client.Request.RequestMarketData(42, new Contract());

            // wait to get the first tickSize message, indicating test start
            await client.Response.OfType<TickSize>().FirstAsync();

            Logger.LogInformation("Receiving...");

            // receive some messages to measure throughput
            var watch = new Stopwatch();
            watch.Start();
            var count = await client.Response.TakeWhile(m => m is TickSize).Count();
            watch.Stop();

            var frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
            Logger.LogInformation($"Received {frequency:N0} messages/second.");

            Logger.LogInformation("Disconnecting.");
            client.Dispose();
            Logger.LogInformation("Disconnected.");
        }
    }
}
