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
        IInterReactClient client = await InterReactClientBuilder.Create()
            .WithLogger(libLogger)
            .WithPorts(port)
            .BuildAsync()
            .ConfigureAwait(false);

        logger.LogInformation("Connected to server.");

        logger.LogInformation("Sending some messages...");
        Enumerable.Range(0, 50).ToList().ForEach(client.Request.CancelMarketData);

        // indicate test end
        client.Request.RequestMarketData(42, new Contract());

        // wait to get the first tickSize message, indicating test start
        await client.Response.OfType<SizeTick>().FirstAsync();

        logger.LogInformation("Receiving...");

        // receive some messages to measure throughput
        Stopwatch watch = new();
        watch.Start();
        int count = await client.Response.TakeWhile(m => m is SizeTick).Count();
        watch.Stop();

        long frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
        logger.LogInformation("Received!! {Frequency:N0} messages/second.", frequency);

        logger.LogInformation("Disconnecting.");
        await client.DisposeAsync();
        logger.LogInformation("Disconnected.");
    }
}
