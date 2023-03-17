using RxSockets;
using System.Net;

namespace ClientServer;

public sealed class Server : IAsyncDisposable
{
    private readonly ILogger Logger;
    private readonly IRxSocketServer SocketServer;
    public IPEndPoint IPEndPoint { get; }

    public Server(ILogger logger, ILogger libLogger)
    {
        Logger = logger;
        
        // Create the server on an available port on the local host.
        SocketServer = RxSocketServer.Create(libLogger);

        // The default EndPoint is type IPEndPoint.
        IPEndPoint = (IPEndPoint)SocketServer.LocalEndPoint;

        SocketServer
            .AcceptAllAsync
            .ToObservableFromAsyncEnumerable()
            .Select(socketClient => Observable.FromAsync(async ct =>
            {
                Logger.LogCritical("Client connection accepted.");
                AcceptClient acceptClient = new(socketClient, Logger);
                await acceptClient.Run();
            }))
            .Concat()
            .Subscribe();

        Logger.LogCritical("Server Started.");
    }

    public async ValueTask DisposeAsync()
    {
        Logger.LogCritical("Disconnecting.");
        await SocketServer.DisposeAsync();
        Logger.LogCritical("Disconnected.");
    }
}
