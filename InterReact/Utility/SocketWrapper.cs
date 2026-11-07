using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace InterReact;

public sealed class SocketWrapper(Socket socket) : IAsyncDisposable
{
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly SemaphoreSlim _receiveLock = new(1, 1);
    internal IPEndPoint RemoteIpEndPoint => (IPEndPoint)socket.RemoteEndPoint!;

    internal async ValueTask WriteStringAsync(string str, CancellationToken ct)
    {
        byte[] bytes = str.ToByteArray();
        await SendAllAsync(bytes, ct).ConfigureAwait(false);
    }

    // V100Plus format: payload of null-terminated strings with 4 byte message length prefix.
    internal async ValueTask WriteMessageAsync(IEnumerable<string> strings, CancellationToken ct)
    {
        byte[] bytes = strings.ToByteArrayWithLengthPrefix();
        await SendAllAsync(bytes, ct).ConfigureAwait(false);
    }

    private async ValueTask SendAllAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct)
    {
        await _sendLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            while (!buffer.IsEmpty)
            {
                // SendAsync is not guaranteed to send the entire buffer in one call
                int sent = await socket.SendAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false);
                if (sent == 0)
                    throw new IOException("Socket closed while sending.");
                buffer = buffer[sent..];
            }
        }
        finally
        {
            _sendLock.Release();
        }
    }

    ///////////////////////////////////////

    internal async ValueTask<string> ReceiveOneNullTerminatedStringAsync(CancellationToken ct)
    {
        using var ms = new MemoryStream();
        byte[] buffer = new byte[1];
        await _receiveLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            while (true)
            {
                // Read one byte at a time until the null terminator is received.
                int received = await socket.ReceiveAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false);
                if (received == 0)
                    throw new EndOfStreamException("Connection closed before null terminator was received.");
                if (buffer[0] == 0)
                    return Encoding.UTF8.GetString(ms.ToArray());
                ms.WriteByte(buffer[0]);
            }
        }
        finally
        {
            _receiveLock.Release();
        }
    }

    internal async ValueTask<string[]> ReceiveOneMessageWithTimoutAsync(TimeSpan timeout, CancellationToken ct)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);
        try
        {
            return await ReceiveOneMessageAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException("Timeout waiting for response from TWS/Gateway. Try restarting.", e);
        }
    }

    internal async ValueTask<string[]> ReceiveOneMessageAsync(CancellationToken ct)
    {
        await _receiveLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            byte[] lengthBuffer = new byte[4]; // Read length prefix
            await ReceiveExactlyAsync(lengthBuffer, ct).ConfigureAwait(false); ;
            int length = BinaryPrimitives.ReadInt32BigEndian(lengthBuffer);
            if (length <= 0 || length > 0x1000)
                throw new InvalidDataException($"Invalid message length: {length}.");
            byte[] payload = new byte[length];
            await ReceiveExactlyAsync(payload, ct).ConfigureAwait(false);
            string[] strings = [.. DecodeStrings(payload)];
            return strings;
        }
        finally
        {
            _receiveLock.Release();
        }
    }

    private async ValueTask ReceiveExactlyAsync(Memory<byte> buffer, CancellationToken ct)
    {
        while (!buffer.IsEmpty)
        {
            int received = await socket.ReceiveAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false);
            if (received == 0)
                throw new EndOfStreamException("Remote endpoint closed connection.");
            buffer = buffer[received..];
        }
    }

    private static string[] DecodeStrings(ReadOnlySpan<byte> bytes)
    {
        List<string> strings = [];
        int start = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == 0)
            {
                strings.Add(Encoding.UTF8.GetString(bytes[start..i]));
                start = i + 1;
            }
        }

        if (start != bytes.Length)
            throw new InvalidDataException("Message does not end with a null-terminated string.");

        return [.. strings];
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await socket.DisconnectAsync(false).ConfigureAwait(false);
        }
        catch { }

        socket.Dispose();
        _sendLock.Dispose();
        _receiveLock.Dispose();
    }
}
