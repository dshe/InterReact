using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CoreClientServer
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(100, 30);
                Console.SetBufferSize(100, 100);
            }

            ConsoleLogger clientLogger = new("Client:    ", ConsoleColor.DarkYellow);
            ConsoleLogger clientLibLogger = new("ClientLib: ", ConsoleColor.DarkGreen);
            ConsoleLogger serverLogger = new("Server:    ", ConsoleColor.DarkMagenta);
            ConsoleLogger serverLibLogger = new("ServerLib: ", ConsoleColor.DarkCyan);

            Server server = new(serverLogger, serverLibLogger);
            int port = server.SocketServer.IPEndPoint.Port;
            Task serverTask = server.Run();

            Task clientTask = Client.Run(port, clientLogger, clientLibLogger);

            await Task.WhenAll(clientTask, serverTask);
            await Task.Delay(1000);

            Console.ResetColor();
        }
    }
}
