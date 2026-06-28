using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Text;
namespace InterReact;

public static partial class Xtensions
{
    extension(Socket socket)
    {
        internal async Task<string> ReadOneStringAsync(CancellationToken ct)
        {
            using var ms = new MemoryStream();
            byte[] buffer = new byte[1];
            while (true)
            {
                int read = await socket.ReceiveAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false);
                if (read == 0)
                    throw new EndOfStreamException();
                if (buffer[0] == 0)
                    break;
                ms.WriteByte(buffer[0]);
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        internal async Task<string[]> ReadOneMessageAsync(CancellationToken ct)
        {
            byte[] header = new byte[4];
            await ReadExactlyAsync(socket, header, ct).ConfigureAwait(false);
            int length = BinaryPrimitives.ReadInt32BigEndian(header);
            if (length <= 0 || length > 4096)
                throw new InvalidDataException();
            byte[] payload = new byte[length];
            await ReadExactlyAsync(socket, payload, ct).ConfigureAwait(false);
            string[] strings = DecodeStrings(payload).ToArray();
            return strings;
        }

        private async Task ReadExactlyAsync(Memory<byte> buffer, CancellationToken ct)
        {
            while (!buffer.IsEmpty)
            {
                int read = await socket.ReceiveAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false);
                if (read == 0)
                    throw new EndOfStreamException();
                buffer = buffer[read..];
            }
        }

        internal async Task RunAsync(IObserver<string[]> observer, ILogger logger, CancellationToken ct)
        {
            logger.LogDebug("Pipeline started.");
            Pipe pipe = new();
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    // Fill pipe from socket.
                    Memory<byte> memory = pipe.Writer.GetMemory(1024);
                    int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None, ct).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;
                    pipe.Writer.Advance(bytesRead);

                    FlushResult flush = await pipe.Writer.FlushAsync(ct).ConfigureAwait(false);
                    if (flush.IsCompleted)
                        break;

                    // Drain pipe immediately.
                    ReadResult result = await pipe.Reader.ReadAsync(ct).ConfigureAwait(false);
                    ReadOnlySequence<byte> buffer = result.Buffer;
                    ReadOnlySequence<byte> remaining = buffer;

                    while (TryReadMessage(ref remaining, out ReadOnlySequence<byte> payload))
                    {
                        string[] strings = payload.IsSingleSegment ? DecodeStrings(payload.FirstSpan) : DecodeStrings(payload.ToArray());
                        observer.OnNext(strings);
                    }
                    pipe.Reader.AdvanceTo(remaining.Start, buffer.End);
                    if (result.IsCompleted)
                        break;
                }

                observer.OnCompleted();
            }
            catch (OperationCanceledException)
            {
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                logger.LogError(ex, "Pipeline failure: {Message}.", ex.Message);
            }
            finally
            {
                logger.LogDebug("Pipeline shutdown.");
                await pipe.Writer.CompleteAsync().ConfigureAwait(false);
                await pipe.Reader.CompleteAsync().ConfigureAwait(false);
            }
        }
    }

    private static bool TryReadMessage(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> payload)
    {
        payload = default;
        SequenceReader<byte> reader = new(buffer);

        if (!reader.TryReadBigEndian(out int length))
            return false;

        if (length <= 0 || length > 4096)
            throw new InvalidDataException();

        if (reader.Remaining < length)
            return false;

        payload = buffer.Slice(reader.Position, length);
        buffer = buffer.Slice(reader.Position).Slice(length);

        return true;
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
            throw new InvalidDataException("Incomplete null-terminated string.");

        return strings.ToArray();
    }

}
