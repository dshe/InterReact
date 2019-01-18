using System;
using System.Threading.Tasks;

namespace CoreClientServer
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.Title = "InterReact";
            Console.SetWindowSize(100, 25);
            Console.SetBufferSize(100, 100);

            const int port = 7777; // arbitrary port
            var serverTask = new Client().Run(port);
            var clientTask = new Server().Run(port);
            await Task.WhenAll(clientTask, serverTask);

            Console.WriteLine(Environment.NewLine + "press a key to exit...");
            Console.ReadKey();
        }

        internal static void Write(string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
    }
}
