namespace ClientServer;

public static class Program
{
    public static async Task Main()
    {
        Console.Title = "InterReact";

        ConsoleLogger clientLogger = new("Client:    ", LogLevel.Trace, ConsoleColor.DarkYellow);
        ConsoleLogger serverLogger = new("Server:    ", LogLevel.Trace, ConsoleColor.DarkMagenta);
        ConsoleLogger clientLibLogger = new("ClientLib: ", LogLevel.Information, ConsoleColor.DarkGreen);
        ConsoleLogger serverLibLogger = new("ServerLib: ", LogLevel.Information, ConsoleColor.DarkCyan);

        Server server = new(serverLogger, serverLibLogger);
        Client client = await Client.CreateAsync(server.IPEndPoint, clientLogger, clientLibLogger);

        await client.MeasurePerformance();
        //client.SendControlMessage("Dispose");
        //client.SendControlMessage("Throw");
        //client.SendControlMessage("Test");

        await Task.Delay(1000);

        await client.DisposeAsync();

        await server.DisposeAsync();

        Console.ResetColor();
    }
}
