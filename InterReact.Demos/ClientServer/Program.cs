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
