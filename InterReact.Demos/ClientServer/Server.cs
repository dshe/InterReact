using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InterReact;
using RxSockets;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CoreClientServer;

internal class Server
{
    public IRxSocketServer SocketServer { get; }
    private readonly ILogger Logger;

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
        string[] messages1 = await accept
            .ReceiveAllAsync()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .FirstAsync();

        // Get the first string of the first message,
        string versions = messages1.Single();

        if (!versions.StartsWith("v"))
            throw new InvalidDataException("Versions not received.");
        Logger.LogCritical($"Received supported server versions: '{versions}'.");

        // Get the second message.
        string[] messages2 = await accept
            .ReceiveAllAsync()
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays()
            .FirstAsync();

        if (messages2[0] != "71")
            throw new InvalidDataException("StartApi message not received.");
        Logger.LogCritical("Received StartApi message.");

        void send(List<string> strings) => accept.Send(strings.ToByteArray().ToByteArrayWithLengthPrefix());

        // Send server version.
        new RequestMessage(send)
            .Write((int)ServerVersion.FRACTIONAL_POSITIONS)
            .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
            .Send();

        // Send managed accounts
        new RequestMessage(send)
            .Write("15")
            .Write("1")
            .Write("123,456,789")
            .Send();

        // Send NextId = 1
        new RequestMessage(send)
            .Write("9")
            .Write("1")
            .Write("10")
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

        RequestMessage message = new(x => send(x));
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
}
