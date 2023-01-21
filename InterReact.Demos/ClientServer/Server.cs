using InterReact;
using Microsoft.Extensions.Logging;
using RxSockets;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CoreClientServer;

internal class Server
{
    private readonly RingLimiter Limiter = new();
    private readonly ILogger Logger;
    public IRxSocketServer SocketServer { get; }
  
    internal Server(ILogger logger, ILogger libLogger)
    {
        Logger = logger;
        SocketServer = RxSocketServer.Create(libLogger);
    }

    internal async Task Run()
    {
        Logger.LogCritical("Waiting for client.");
        IRxSocketClient accept = await SocketServer.AcceptAllAsync().FirstAsync();
        Logger.LogCritical("Client connection accepted.");

        string firstString = await accept.ReceiveAllAsync().ToStrings().FirstAsync();  

        if (firstString != "API")
            throw new InvalidDataException("'API' not received.");
        Logger.LogCritical("Received 'API'.");

        // Start receiving messages with length prefix.
        // Get the first message (string array).
        string[] message1 = await GetMessage(accept);

        // Get the first string of the first message,
        string versions = message1.Single();

        if (!versions.StartsWith("v"))
            throw new InvalidDataException("Versions not received.");
        Logger.LogCritical("Received supported server versions: '{Versions}'.", versions);

        // Get the second message.
        string[] message2 = await GetMessage(accept);
   
        if (message2[0] != "71")
            throw new InvalidDataException("StartApi message not received.");
        Logger.LogCritical("Received StartApi message.");

        // Send server version.
        new RequestMessage(accept, Limiter)
            .Write((int)ServerVersion.FRACTIONAL_POSITIONS)
            .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
            .Send();

        // Send NextOrderId = 10
        new RequestMessage(accept, Limiter)
            .Write("9")
            .Write("1")
            .Write("10")
            .Send();

        // Send managed accounts
        new RequestMessage(accept, Limiter)
            .Write("15")
            .Write("1")
            .Write("123,456,789")
            .Send();
    
        Logger.LogCritical("Client login complete.");

        ////////////////////////////////////////////////////

        IObservable<string[]> obs = accept
            .ReceiveAllAsync()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .ToObservableFromAsyncEnumerable()
            .Publish()
            .AutoConnect();

        // receive test start signal
        await obs.FirstAsync();

        Stopwatch watch = new();
        watch.Start();

        int count = await obs.TakeWhile(m => m[0] == "2").Count();

        watch.Stop();

        long frequency = Stopwatch.Frequency * (count + 1) / watch.ElapsedTicks;
        Logger.LogCritical($"Received {frequency:N0} messages/second.");

        RequestMessage message = new(accept, Limiter);
        for (int i = 0; i < 30_000; i++)
            message.Write("2", "3", 1, TickType.LastSize, 300).Send();
        message.Write("1", "3", 1, TickType.LastPrice, 100, 200, true).Send();

        Logger.LogCritical("Sending some messages...");

        // wait for OnCompleted()
        await obs.LastOrDefaultAsync();

        Logger.LogCritical("Disconnecting.");
        await SocketServer.DisposeAsync();
        Logger.LogCritical("Disconnected.");
    }

    internal static async Task<string[]> GetMessage(IRxSocketClient client)
    {
        return await client
            .ReceiveAllAsync()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .FirstAsync();
    }
}
