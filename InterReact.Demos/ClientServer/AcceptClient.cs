using Microsoft.Extensions.Logging.Abstractions;
using RxSockets;
using System.Diagnostics;
using System.IO;

namespace ClientServer;

public sealed class AcceptClient
{
    private readonly ILogger Logger;
    private readonly IRxSocketClient SocketClient;
    private readonly IObservable<ServerRequestMessage> ServerRequestMessages;
    private readonly RequestMessage ServerResponseMessage;

    public AcceptClient(IRxSocketClient socketClient, ILogger logger)
    {
        Logger = logger;
        SocketClient = socketClient;
        ServerResponseMessage = new RequestMessage(NullLoggerFactory.Instance, socketClient, new RingLimiter());
        ServerRequestMessages = socketClient
                .ReceiveAllAsync
                .ToObservableFromAsyncEnumerable()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .ToServerRequestMessages()
                .Publish()
                .AutoConnect();
    }

    public async Task Run()
    {
        await Login();

        ServerRequestMessages
            .Select(message => Observable.FromAsync(async ct =>
            {
                if (message.RequestCode == RequestCode.Invalid)
                    throw new Exception("Invalid RequestCode.");

                if (message.RequestCode == RequestCode.Control)
                {
                    string msg = message.Strings.Single();
                    if (msg == "Measure Performance")
                        await MeasurePerformance();
                    else if (msg == "Throw")
                        throw new InvalidOperationException("test");
                    else if (msg == "Dispose")
                        await SocketClient.DisposeAsync();
                    else if (msg == "Test")
                        ServerResponseMessage.Write("xxx").Send();
                }
            }))
            .Concat()
            .Subscribe();
    }

    private async Task Login()
    {
        // Receiving one string without length prefix.
        string firstString = await SocketClient
            .ReceiveAllAsync
            .ToStrings()
            .FirstAsync();

        if (firstString != "API")
            throw new InvalidDataException("'API' not received.");
        Logger.LogCritical("Received 'API'.");

        // Start receiving messages with length prefix.
        IAsyncEnumerable<string[]> messages = SocketClient
            .ReceiveAllAsync
            .ToArraysFromBytesWithLengthPrefix()
            .ToStringArrays();

        // Get the first message (string array).
        string[] message1 = await messages.FirstAsync();

        // Get the first string of the first message,
        string versions = message1.Single();

        if (!versions.StartsWith("v"))
            throw new InvalidDataException("Versions not received.");
        Logger.LogCritical("Received supported server versions: '{Versions}'.", versions);

        // Get the second message.
        string[] message2 = await messages.FirstAsync();

        if (message2[0] != "71")
            throw new InvalidDataException("StartApi message not received.");
        Logger.LogCritical("Received StartApi message.");

        // Send server version and date.
        ServerResponseMessage
            .Write((int)ServerVersion.MIN_SERVER_VER_BOND_ISSUERID)
            .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
            .Send();

        // Send NextOrderId = 1.
        ServerResponseMessage
            .Write("9")
            .Write("1")
            .Write("1")
            .Send();

        // Send managed accounts
        ServerResponseMessage
            .Write("15")
            .Write("1")
            .Write("123,456,789")
            .Send();

        Logger.LogCritical("Client login complete.");
    }

    public async Task MeasurePerformance()
    {
        Stopwatch watch = new();

        watch.Start();

        int count = await ServerRequestMessages.TakeWhile(m => m.RequestCode != RequestCode.Control).Count();
        watch.Stop();
        long frequency = Stopwatch.Frequency * (count + 1) / watch.ElapsedTicks;
        Logger.LogCritical("Received {frequency:N0} messages/second.", frequency);

        Logger.LogCritical("Sending some messages...");
        // Send size ticks.
        for (int i = 0; i < 100_000; i++)
            ServerResponseMessage.Write("2", "3", 1, TickType.LastSize, 300).Send();

        // Indicate test stop by sending a different response.
        ServerResponseMessage.Write("1", "3", 1, TickType.LastPrice, 100, 200, true).Send();
    }
}
