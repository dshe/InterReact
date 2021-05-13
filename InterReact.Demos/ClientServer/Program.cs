using System;
using System.Threading.Tasks;

namespace CoreClientServer
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";
#pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(100, 30);
            Console.SetBufferSize(100, 100);
#pragma warning restore CA1416 // Validate platform compatibility

            var clientLogger = new ConsoleLogger("Client:    ", ConsoleColor.DarkYellow);
            var clientLibLogger = new ConsoleLogger("ClientLib: ", ConsoleColor.DarkGreen);
            var serverLogger = new ConsoleLogger("Server:    ", ConsoleColor.DarkMagenta);
            var serverLibLogger = new ConsoleLogger("ServerLib: ", ConsoleColor.DarkCyan);

            var server = new Server(serverLogger, serverLibLogger);
            var port = server.SocketServer.IPEndPoint.Port;
            var serverTask = server.Run();

            var clientTask = Client.Run(port, clientLogger, clientLibLogger);

            await Task.WhenAll(clientTask, serverTask);
            await Task.Delay(1000);

            Console.ResetColor();
        }
    }
}
