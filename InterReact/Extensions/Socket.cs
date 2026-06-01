using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Text;
namespace InterReact;

public static partial class Extension
{
    internal static IObservable<string[]> CreateObservable(this Socket socket)
    {
        return Observable.Create<string[]>(observer =>
        {
            CancellationTokenSource cts = new();
            int terminated = 0;

            _ = Task.Run(async () =>
            {
                try
                {
                    await RunAsync(socket, observer, cts.Token).ConfigureAwait(false);
                    Complete();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Error(ex);
                }
                finally
                {
                }
                void Complete()
                {
                    if (TryTerminate())
                        observer.OnCompleted();
                }
                void Error(Exception ex)
                {
                    if (TryTerminate())
                        observer.OnError(ex);
                }
                bool TryTerminate() => Interlocked.Exchange(ref terminated, 1) == 0;
            });
            return Disposable.Create(() => cts.Cancel()); // don't bother disposing cts
        });
    }

    private static async Task RunAsync(Socket socket, IObserver<string[]> observer, CancellationToken ct)
    {
        Pipe pipe = new();
        Task writing = FillPipeAsync(socket, pipe.Writer, ct);
        Task reading = ReadPipeAsync(pipe.Reader, observer, ct);
        await Task.WhenAll(writing, reading).ConfigureAwait(false);
    }

    private static async Task FillPipeAsync(Socket socket, PipeWriter writer, CancellationToken ct)
    {
        const int minimumBufferSize = 1024;
        try
        {
            while (true)
            {
                Memory<byte> memory = writer.GetMemory(minimumBufferSize);
                int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None, ct).ConfigureAwait(false);
                if (bytesRead == 0)
                    break;
                writer.Advance(bytesRead);
                try
                {
                    FlushResult result = await writer.FlushAsync(ct).ConfigureAwait(false);

                    if (result.IsCompleted)
                        break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        finally
        {
            await writer.CompleteAsync().ConfigureAwait(false);
        }
    }

    private static async Task ReadPipeAsync(PipeReader reader, IObserver<string[]> observer, CancellationToken ct)
    {
        try
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync(ct).ConfigureAwait(false);

                ReadOnlySequence<byte> buffer = result.Buffer;
                ReadOnlySequence<byte> remaining = buffer;

                while (TryReadMessage(ref remaining, out ReadOnlySequence<byte> payload))
                    observer.OnNext(DecodeStrings(payload.ToArray()).ToArray());

                reader.AdvanceTo(remaining.Start, remaining.End);

                if (result.IsCompleted)
                {
                    if (!remaining.IsEmpty)
                        throw new InvalidDataException("Incomplete message.");
                    break;
                }
            }
        }
        finally
        {
            await reader.CompleteAsync().ConfigureAwait(false);
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

    private static IEnumerable<string> DecodeStrings(byte[] bytes)
    {
        int start = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] != 0)
                continue;
            yield return Encoding.UTF8.GetString(bytes, start, i - start);
            start = i + 1;
        }

        if (start != bytes.Length)
            throw new InvalidDataException("Incomplete null-terminated string.");
    }

    internal static async Task<string> ReadStringAsync(this Socket socket, CancellationToken ct)
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

    internal static async Task<string[]> ReadMessageAsync(this Socket socket, CancellationToken ct)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        try
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
        catch (OperationCanceledException e) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException("Timeout waiting for response from TWS/Gateway. Try restarting.", e);
        }
    }
    private static async Task ReadExactlyAsync(Socket socket, Memory<byte> buffer, CancellationToken ct)
    {
        while (!buffer.IsEmpty)
        {
            int read = await socket.ReceiveAsync(buffer, SocketFlags.None, ct).ConfigureAwait(false); ;
            if (read == 0)
                throw new EndOfStreamException();
            buffer = buffer[read..];
        }
    }
}
