using System.Net;
namespace ClientServer;

public static partial class Program
{
    public static async Task Main()
    {
        Console.Title = "InterReact";

        ConsoleLogger clientLogger    = new("Client:     ", LogLevel.Trace, ConsoleColor.DarkYellow);
        ConsoleLogger clientLibLogger = new("InterReact: ", LogLevel.Information, ConsoleColor.Magenta);
        ConsoleLogger serverLogger    = new("Server:     ", LogLevel.Trace, ConsoleColor.DarkCyan);

        IPEndPoint endPoint = new(IPAddress.Loopback, 111);
        
        _ = Task.Run(() => RunServerAsync(endPoint, serverLogger));
        await Task.Delay(100);

        await RunClientAsync(endPoint, clientLogger, clientLibLogger);

        await Console.In.ReadLineAsync();

        Console.ResetColor();
    }
}
