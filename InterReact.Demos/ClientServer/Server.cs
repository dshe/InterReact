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
    public static async Task RunServerAsync(IPEndPoint endPoint, ILogger logger, CancellationToken ct)
    {
        CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        Socket serverSocket = new(SocketType.Stream, ProtocolType.Tcp);
        try
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen(1);
            logger.LogCritical("Server started, waiting to accept a connection.");

            Socket clientSocket = serverSocket.Accept();
            logger.LogCritical("Client connection accepted.");

            string str = await clientSocket.ReadOneStringAsync(ct);
            logger.LogCritical("Received string: {0}", str.Stringify(false));
            Trace.Assert(str == "API");

            string[] message1 = await clientSocket.ReadOneMessageAsync(ct);
            logger.LogCritical("Received message: {0}", message1.Stringify(false));
            Trace.Assert(message1.Length == 1);

            string[] message2 = await clientSocket.ReadOneMessageAsync(ct);
            logger.LogCritical("Received message: {0}", message2.Stringify(false));
            Trace.Assert(message2.Length == 4);

            await clientSocket.WriteMessageAsync(ServerVersion.BOND_ISSUERID.ToString(), "the date");

            await clientSocket.WriteMessageAsync("9", "0", "1"); // NextId Message

            logger.LogCritical("Handshake completed.");

            IObservable<string[]> observable = clientSocket.CreateObservable();
            observable.Subscribe(
                x => logger.LogCritical("Received: " + x.Stringify(false)),
                ex =>
                {
                    logger.LogError(ex, "Socket error: " + ex.Message);
                    cts.Cancel();
                },
                () =>
                {
                    logger.LogInformation("Disconnecting.");
                    cts.Cancel();
                }
            );

            await Task.Delay(1000, ct);

            await clientSocket.WriteMessageAsync("1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.01", "200", "0");
            await clientSocket.WriteMessageAsync("1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.02", "200", "0");
            await clientSocket.WriteMessageAsync("1", "3", "1", ((int)TickType.LastPrice).ToString(), "10.03", "200", "0");

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
