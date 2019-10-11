using RxSockets;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#nullable enable

namespace CoreClientServer
{
    public static class Program
    {
        const int Port = 7777; // use an arbitrary port

        public static async Task Main()
        {
            Console.Title = "InterReact";
            Console.SetWindowSize(100, 30);
            Console.SetBufferSize(100, 100);

            var serverLoggerFactory = new MyLoggerFactory("Server: ", ConsoleColor.DarkMagenta);
            var clientLoggerFactory = new MyLoggerFactory("Client: ", ConsoleColor.DarkYellow);
            var libraryLoggerFactory = new MyLoggerFactory("ClientLibrary: ", ConsoleColor.DarkCyan);

            var serverTask = new Client(Port, clientLoggerFactory.CreateLogger(), libraryLoggerFactory).Run();
            var clientTask = new Server(Port, serverLoggerFactory.CreateLogger<RxSocketServer>()).Run();

            await Task.WhenAll(clientTask, serverTask);
            await Task.Delay(1000);

            Console.ResetColor();
        }
    }
}
