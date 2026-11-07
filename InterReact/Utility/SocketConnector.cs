using System.Net;
using System.Net.Sockets;
namespace InterReact;

internal class SocketConnector
{
    internal static async Task<Socket> ConnectAsync(IPAddress ipAddress, IReadOnlyList<int> ports, ILogger logger, CancellationToken ct)
    {
        ///IPAddress ipAddress = _options.TwsIpAddress;
        //IReadOnlyList<int> ports = _options.TwsPortAddresses;
        if (ports.Count == 0)
            throw new InvalidOperationException("No ports specified.");

        foreach (int port in ports)
        {
            Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(3));
                await socket.ConnectAsync(ipAddress, port, cts.Token).ConfigureAwait(false);
                logger.LogInformation("Connected to TWS/Gateway at {IpAddress}:{Port}.", ipAddress, port);
                return socket;
                // ct cancel  => OperationCanceledException
                // cts timeout => OperationCanceledException
                // socket timeout/error => SocketException
            }
            catch
            {
                // don't reuse the Socket after a failed connect attempt
                socket.Dispose();
                if (ct.IsCancellationRequested)
                    throw;
                // timeout -> try next port
            }
        }
        string message = $"Could not connect to TWS/Gateway at " +
            $"[{ipAddress}]:{string.Join(", ", ports)}. " +
            $"In TWS, navigate to Edit / Global Configuration / API / Settings and ensure" +
            $" the option \"Enable ActiveX and Socket Clients\" is selected.";
        logger.LogCritical("{Message}", message);
        throw new InvalidOperationException(message);
    }
}
