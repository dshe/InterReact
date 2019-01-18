using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Utility;
using RxSockets;
using System.Reactive.Linq;

namespace CoreClientServer
{
    internal class Server
    {
        private static void Write(string text) => Program.Write("Server: " + text, ConsoleColor.DarkCyan);

        internal async Task Run(int port)
        {
            var server = RxSocketServer.Create(port);
            var limiter = new Limiter();

            var accept = await server.AcceptObservable.FirstAsync();
            Write("Connection accepted.");

            var firstString = await accept.ReceiveObservable.ToStrings().FirstAsync();
            if (firstString != "API")
                throw new InvalidDataException("'API' not received.");
            Write("Received 'API'.");

            // Start receiving with length prefix.
            var messages = await accept.ReceiveObservable.ToByteArrayOfLengthPrefix().ToStringArray().Take(2).ToList();

            var versions = messages[0].Single();
            if (!versions.StartsWith("v"))
                throw new InvalidDataException("Versions not received.");
            Write($"Received supported server versions: '{versions}'.");

            if (messages[1][0] != "71") // receive StartApi message
                throw new InvalidDataException("StartApi message not received.");
            Write("Received StartApi message.");

            new RequestMessage(accept, limiter)
                .Write(75) // server version
                .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
                .Send();

            new RequestMessage(accept, limiter)
                .Write("15")
                .Write("1")
                .Write("123,456,789")
                .Send();

            new RequestMessage(accept, limiter)
                .Write("9")
                .Write("1")
                .Write("10")
                .Send();

            Write("Client login complete.");

            ////////////////////////////////////////////////////

            var obs = accept.ReceiveObservable.ToByteArrayOfLengthPrefix().ToStringArray().Publish().RefCount();

            // receive test start signal
            await obs.FirstAsync();

            var watch = new Stopwatch();
            watch.Start();
            var count = await obs.TakeWhile(m => m[0] == "2").Count();
            watch.Stop();
            var frequency = Stopwatch.Frequency * (count + 1) / watch.ElapsedTicks;
            Write($"Received {frequency:N0} messages/second.");

            var ms = new MemoryStream();
            for (var i = 0; i < 500_000; i++)
                new RequestMessage(ms.Write, limiter)
                .Write("2", "3", 1, TickType.LastSize, 300)
                .Send();

            // message to indicate test stop
            new RequestMessage(ms.Write, limiter)
                .Write("1", "3", 1, TickType.LastPrice, 100, 200, true)
                .Send();

            Write("Sending messages...");
            accept.Send(ms.ToArray(), 0, (int)ms.Position);

            // wait for OnCompleted()
            await obs.LastOrDefaultAsync();

            Write("Disconnecting.");
            await accept.DisconnectAsync();
            await server.DisconnectAsync();
            Write("Disconnected.");
        }
    }
}
