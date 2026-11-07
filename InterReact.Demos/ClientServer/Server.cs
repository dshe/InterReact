using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace ClientServer;

public static partial class Program
{
    public static async Task RunServerAsync(IPEndPoint endPoint, ILogger logger, CancellationToken ct)
    {
        Socket serverSocket = new(SocketType.Stream, ProtocolType.Tcp);
        SocketWrapper? clientSocketWrapper = null;
        try
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen(1);
            logger.LogCritical("Server started, waiting to accept a connection.");

            Socket clientSocket = serverSocket.Accept();
            clientSocketWrapper = new(clientSocket);
            logger.LogCritical("Client connection accepted.");

            string str = await clientSocketWrapper.ReceiveOneNullTerminatedStringAsync(ct);
            logger.LogCritical("Received: [{0}]", str.Stringify(false));
            Trace.Assert(str == "API");

            string[] message1 = await clientSocketWrapper.ReceiveOneMessageAsync(ct);
            logger.LogCritical("Received: {0}", message1.Stringify(false));
            Trace.Assert(message1.Length == 1);

            string[] message2 = await clientSocketWrapper.ReceiveOneMessageAsync(ct);
            logger.LogCritical("Received: {0}", message2.Stringify(false));
            Trace.Assert(message2.Length == 4);

            await clientSocketWrapper.WriteMessageAsync([ServerVersion.BOND_ISSUERID.ToString(), "the date"], ct);
            await clientSocketWrapper.WriteMessageAsync(["9", "0", "1"], ct); // NextId Message

            logger.LogCritical("Handshake completed.");

            IObservable<string[]> observable = Observable.Create<string[]>(async observer =>
            {
                logger.LogInformation("ConnectionObservable Startup.");
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        string[] message = await clientSocketWrapper.ReceiveOneMessageAsync(ct).ConfigureAwait(false);
                        observer.OnNext(message);
                    }
                    observer.OnCompleted();
                }
                catch (EndOfStreamException) // when (ct.IsCancellationRequested)
                {
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            });

            observable.Subscribe(
                x => logger.LogCritical("Received: " + x.Stringify(false)),
                ex =>
                {
                    logger.LogError(ex, "Socket error: " + ex.Message);
                },
                () =>
                {
                    logger.LogInformation("Disconnecting.");
                }
            );

            await Task.Delay(TimeSpan.FromSeconds(1), ct);

            await clientSocketWrapper.WriteMessageAsync(["1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.01", "200", "0"], ct);
            await clientSocketWrapper.WriteMessageAsync(["1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.02", "200", "0"], ct);
            await clientSocketWrapper.WriteMessageAsync(["1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.03", "200", "0"], ct);

            await Task.Delay(Timeout.Infinite, ct);
        }
        catch (TaskCanceledException)
        {
        }
        catch (EndOfStreamException)
        {
            logger.LogCritical("EOS.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
        finally
        {
            if (clientSocketWrapper != null)
                await clientSocketWrapper.DisposeAsync();
            serverSocket.Dispose();
            logger.LogCritical("Disposed.");
        }
    }
}
