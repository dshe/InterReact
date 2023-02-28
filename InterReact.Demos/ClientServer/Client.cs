using System.Diagnostics;

namespace ClientServer;

public sealed class Client : IAsyncDisposable
{
    private readonly ILogger Logger;
    private readonly IInterReactClient IRClient;

    private Client(IInterReactClient irClient, ILogger logger)
    {
        IRClient = irClient;
        Logger = logger;
    }

    public static async Task<Client> CreateAsync(int port, ILogger logger, ILogger libLogger)
    {
        IInterReactClient irClient = await new InterReactClientConnector()
            .WithLoggerFactory(libLogger.ToLoggerFactory())
            .WithPort(port)
            .ConnectAsync();

        logger.LogCritical("Connected to server.");

        return new Client(irClient, logger);
    }

    public void SendControlMessage(string message) => IRClient.Request.RequestControl(message);

    public async Task MeasurePerformance()
    {
        SendControlMessage("Measure Performance");

        Logger.LogCritical("Sending some messages...");
        Enumerable.Range(0, 50).ToList().ForEach(IRClient.Request.CancelMarketData);

        // Indicate test stop by sending any other message
        IRClient.Request.RequestControl("End Test");
        //Client.Request.CancelNewsBulletins();

        // wait to get the first tickSize message, indicating receive test start
        await IRClient.Response.OfType<SizeTick>().FirstAsync();

        Logger.LogCritical("Receiving...");

        // receive some messages to measure throughput
        Stopwatch watch = new();
        watch.Start();
        int count = await IRClient.Response.TakeWhile(m => m is SizeTick).Count();
        watch.Stop();

        long frequency = Stopwatch.Frequency * count / watch.ElapsedTicks;
        Logger.LogCritical("Received {Frequency:N0} messages/second.", frequency);

        // send another message to signal test end.
        IRClient.Request.RequestMarketData(42, new Contract());
    }

    public async ValueTask DisposeAsync()
    {
        Logger.LogCritical("Disconnecting.");
        await IRClient.DisposeAsync();
        Logger.LogCritical("Disconnected.");
    }
}
