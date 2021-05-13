using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.Utility;
using RxSockets;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace CoreClientServer
{
    internal class Server
    {
        public IRxSocketServer SocketServer { get; }
        private readonly ILogger Logger;
        private readonly Limiter Limiter = new Limiter();

        internal Server(ILogger logger, ILogger libLogger)
        {
            Logger = logger;
            SocketServer = RxSocketServer.Create(libLogger);
        }

        internal async Task Run()
        {
            Logger.LogInformation("Waiting for client.");
            var accept = await SocketServer.AcceptObservable.FirstAsync();
            Logger.LogInformation("Client connection accepted.");

            var firstString = await accept.ReadAsync().ToStringAsync();

            if (firstString != "API")
                throw new InvalidDataException("'API' not received.");
            Logger.LogInformation("Received 'API'.");

            // Start receiving with length prefix.
            var messages1 = await accept.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync();
            var versions = messages1.Single();

            if (!versions.StartsWith("v"))
                throw new InvalidDataException("Versions not received.");
            Logger.LogInformation($"Received supported server versions: '{versions}'.");

            var messages2 = await accept.ReadAsync().ToStringsFromBufferWithLengthPrefixAsync();

            if (messages2[0] != "71") // receive StartApi message
                throw new InvalidDataException("StartApi message not received.");
            Logger.LogInformation("Received StartApi message.");

            new RequestMessage(accept.Send, Limiter)
                .Write(149) // server version
                .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
                .Send();

            new RequestMessage(accept.Send, Limiter)
                .Write("15")
                .Write("1")
                .Write("123,456,789")
                .Send();

            new RequestMessage(accept.Send, Limiter)
                .Write("9")
                .Write("1")
                .Write("10")
                .Send();

            Logger.LogInformation("Client login complete.");

            ////////////////////////////////////////////////////

            var obs = accept.ReceiveObservable
                .RemoveLengthPrefix()
                .ToStrings()
                .Publish().RefCount();

            // receive test start signal
            await obs.FirstAsync();

            var watch = new Stopwatch();
            watch.Start();
            var count = await obs.TakeWhile(m => m[0] == "2").Count();
            watch.Stop();
            var frequency = Stopwatch.Frequency * (count + 1) / watch.ElapsedTicks;
            Logger.LogInformation($"Received {frequency:N0} messages/second.");

            var ms = new MemoryStream();
            for (var i = 0; i < 500_000; i++)
                new RequestMessage(ms.Write, Limiter)
                .Write("2", "3", 1, TickType.LastSize, 300)
                .Send();

            // message to indicate test stop
            new RequestMessage(ms.Write, Limiter)
                .Write("1", "3", 1, TickType.LastPrice, 100, 200, true)
                .Send();

            Logger.LogInformation("Sending messages...");
            accept.Send(ms.ToArray(), 0, (int)ms.Position);

            // wait for OnCompleted()
            await obs.LastOrDefaultAsync();

            Logger.LogInformation("Disconnecting.");
            await SocketServer.DisposeAsync();
            Logger.LogInformation("Disconnected.");
        }
    }
}
