using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace ClientServer;

public static partial class Program
{
    public static async Task RunServerAsync(IPEndPoint endPoint, ILogger logger)
    {
        Socket serverSocket = new(SocketType.Stream, ProtocolType.Tcp);
        try
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen(1);
            logger.LogCritical("Server started, waiting to accept a connection.");

            Socket clientSocket = serverSocket.Accept();
            logger.LogCritical("Client connection accepted.");

            string str = await clientSocket.ReadStringAsync(default);
            Trace.Assert(str == "API");

            string[] message1 = await clientSocket.ReadMessageAsync(default);
            Trace.Assert(message1.Length == 1);

            string[] message2 = await clientSocket.ReadMessageAsync(default);
            Trace.Assert(message2.Length == 4);

            await clientSocket.WriteMessageAsync(ServerVersion.BOND_ISSUERID.ToString(), "the date");
            logger.LogCritical("Handshake completed.");

            CancellationTokenSource cts = new();

            IObservable<string[]> observable = clientSocket.CreateObservable();
            observable.Subscribe(
                // OnNext
                x => logger.LogCritical("Client received: " + string.Join(", ", x)),
                ex =>
                {   // OnError
                    logger.LogError(ex, "Socket error: " + ex.Message);
                    cts.Cancel();
                },
                () =>
                {   // OnCompleted
                    logger.LogInformation("Disconnecting.");
                    cts.Cancel();
                }
            );

            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
        finally
        {
            serverSocket.Dispose();
            logger.LogCritical("Disposed.");
        }
    }

    private static async Task WriteMessageAsync(this Socket socket, params string[] message)
    {
        // Encode each string as UTF-8 with a null terminator
        using var ms = new MemoryStream();
        foreach (string s in message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            ms.Write(bytes, 0, bytes.Length);
            ms.WriteByte(0); // null terminator
        }

        byte[] payload = ms.ToArray();

        // Write length prefix (big-endian 4-byte integer)
        byte[] header = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(header, payload.Length);

        // Send header then payload
        await socket.SendAsync(header, SocketFlags.None).ConfigureAwait(false);
        await socket.SendAsync(payload, SocketFlags.None).ConfigureAwait(false);
    }
}
