﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InterReact;
using RxSockets;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace CoreClientServer
{
    internal class Server
    {
        public IRxSocketServer SocketServer { get; }
        private readonly ILogger Logger;
        private readonly Limiter Limiter = new();

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

            var firstString = await accept.ReceiveAllAsync().ToStrings().FirstAsync();

            if (firstString != "API")
                throw new InvalidDataException("'API' not received.");
            Logger.LogInformation("Received 'API'.");

            // Start receiving messages with length prefix.
            // Get the first message (string array).
            var messages1 = await accept
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync();

            // Get the first string of the first message,
            var versions = messages1.Single();

            if (!versions.StartsWith("v"))
                throw new InvalidDataException("Versions not received.");
            Logger.LogInformation($"Received supported server versions: '{versions}'.");

            // Get the second message.
            var messages2 = await accept
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .FirstAsync();

            if (messages2[0] != "71")
                throw new InvalidDataException("StartApi message not received.");
            Logger.LogInformation("Received StartApi message.");

            // Send server version.
            new RequestMessage(accept, Limiter)
                .Write(157) 
                .Write(DateTime.Now.ToString("yyyyMMdd HH:mm:ss XXX"))
                .Send();

            // Send managed accounts
            new RequestMessage(accept, Limiter)
                .Write("15")
                .Write("1")
                .Write("123,456,789")
                .Send();

            // Send NextId = 1
            new RequestMessage(accept, Limiter)
                .Write("9")
                .Write("1")
                .Write("10")
                .Send();

            Logger.LogInformation("Client login complete.");

            ////////////////////////////////////////////////////

            var obs = accept
                .ReceiveAllAsync()
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays()
                .ToObservableFromAsyncEnumerable()
                .Publish()
                .AutoConnect();

            // receive test start signal
            await obs.FirstAsync();

            var watch = new Stopwatch();
            watch.Start();

            var count = await obs.TakeWhile(m => m[0] == "2").Count();
            
            watch.Stop();
            
            var frequency = Stopwatch.Frequency * (count + 1) / watch.ElapsedTicks;
            Logger.LogInformation($"Received {frequency:N0} messages/second.");

            var ms = new MemoryStream();
            for (var i = 0; i < 100_000; i++)
            {
                ms.Write(
                new RequestMessage(accept, Limiter)
                    .Write("2", "3", 1, TickType.LastSize, 300)
                    .Get());
            }
            ms.Write(
            new RequestMessage(accept, Limiter)
                .Write("1", "3", 1, TickType.LastPrice, 100, 200, true)
                .Get());

            accept.Send(ms.ToArray());

            Logger.LogInformation("Sending some messages...");

            // wait for OnCompleted()
            await obs.LastOrDefaultAsync();

            Logger.LogInformation("Disconnecting.");
            await SocketServer.DisposeAsync();
            Logger.LogInformation("Disconnected.");
        }
    }
}
