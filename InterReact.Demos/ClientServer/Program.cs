using System.Net;
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
        
        _ = Task.Run(() => RunServerAsync(endPoint, serverLogger, default));
        await Task.Delay(100);

        await RunClientAsync(endPoint, clientLogger, clientLibLogger);

        await Console.In.ReadLineAsync();

        Console.ResetColor();
    }
}
