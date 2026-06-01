using System.Diagnostics;
using System.Net;
namespace ClientServer;

public static partial class Program
{
    public static async Task RunClientAsync(IPEndPoint endPoint, ILogger logger, ILogger libLogger)
    {
        logger.LogCritical("Connecting.");

        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.Logger = libLogger;
            options.TwsIpAddress = endPoint.Address;
            options.TwsPortAddresses = [endPoint.Port];
        });

        logger.LogCritical("Connected.");

        int id = client.Request.GetNextId();
        logger.LogInformation("NextId = {Id}.", id);

        IDisposable subscription = client.Response.Subscribe(
            // OnNext
            x => logger.LogCritical("Received: " + string.Join(", ", x) + "."),
            ex =>
            {   // OnError
                logger.LogError(ex, "Socket error.");
            },
            () =>
            {   // OnCompleted
                logger.LogInformation("Socket disconnected.");
            }
        );

        await Task.Delay(100);

        subscription.Dispose();
        logger.LogInformation("Disconnected.");

        await client.DisposeAsync();
        logger.LogInformation("Disposed.");
    }
}


public sealed class Client
{
    //private readonly ILogger Logger;
    //private readonly IInterReactClient IRClient;

    //public async Task SendControlMessage(string message) => await IRClient.Request.RequestControl(message);

    public async Task MeasurePerformance()
    {
        //Logger.LogCritical("Sending some messages...");
        //Enumerable.Range(0, 50).ToList().ForEach(await Client.Request.CancelMarketDataAsync());

        // Indicate test stop by sending any other message
        //IRClient.Request.RequestControl("End Test");
        //Client.Request.CancelNewsBulletins();

        // wait to get the first tickSize message, indicating receive test start
        //await IRClient.Response.OfType<SizeTick>().FirstAsync();

        //Logger.LogCritical("Receiving...");

        // receive some messages to measure throughput
        //Stopwatch watch = new();
        //watch.Start();
        //int count = await IRClient.Response.TakeWhile(m => m is SizeTick).Count();
        //watch.Stop();

        //long frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
        //Logger.LogCritical("Received {Frequency:N0} messages/second.", frequency);

        // send another message to signal test end.

    }
}
