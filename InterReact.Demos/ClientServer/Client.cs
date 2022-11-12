using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using Microsoft.Extensions.Logging;

namespace CoreClientServer;

internal static class Client
{
    internal static async Task Run(int port, ILogger logger, ILogger libLogger)
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLogger(libLogger)
            .WithPort(port)
            .ConnectAsync()
            .ConfigureAwait(false);

        logger.LogCritical("Connected to server.");

        logger.LogCritical("Sending some messages...");
        Enumerable.Range(0, 50).ToList().ForEach(client.Request.CancelMarketData);

        // indicate test end
        client.Request.RequestMarketData(42, new Contract());

        // wait to get the first tickSize message, indicating test start
        await client.Response.OfType<SizeTick>().FirstAsync();

        logger.LogCritical("Receiving...");

        // receive some messages to measure throughput
        Stopwatch watch = new();
        watch.Start();
        int count = await client.Response.TakeWhile(m => m is SizeTick).Count();
        watch.Stop();

        long frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
        logger.LogCritical("Received!! {Frequency:N0} messages/second.", frequency);

        logger.LogCritical("Disconnecting.");
        await client.DisposeAsync();
        logger.LogCritical("Disconnected.");
    }
}
