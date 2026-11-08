using System.Net;
using System.Threading;
namespace ClientServer;

public static partial class Program
{
    public static async Task Main()
    {
        Console.Title = "InterReact";

        ConsoleLogger clientLogger    = new("Client:     ", LogLevel.Trace, ConsoleColor.DarkMagenta);
        ConsoleLogger clientLibLogger = new("InterReact: ", LogLevel.Information, ConsoleColor.DarkGreen);
        ConsoleLogger serverLogger    = new("Server:     ", LogLevel.Trace, ConsoleColor.Blue);

        IPEndPoint endPoint = new(IPAddress.Loopback, 111);

        CancellationTokenSource serverCt = new();
        _ = Task.Run(() => RunServerAsync(endPoint, serverLogger, serverCt.Token));
        await Task.Delay(500);

        await RunClientAsync(endPoint, clientLogger, clientLibLogger);

        await serverCt.CancelAsync();

        await Task.Delay(500);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\npress a key to continue...");
        await Console.In.ReadLineAsync();

        Console.ResetColor();
    }
}
