namespace InterReact;

internal sealed class AsyncSemaphore : IDisposable
{
    private readonly Semaphore Semaphore;

    internal AsyncSemaphore(string name , int initialCount = 1, int maximumCount = 1)
    {
        Semaphore = new Semaphore(initialCount, maximumCount, name);
    }

    public async Task<bool> WaitAsync(CancellationToken ct = default)
    {
        bool success = await Task.Run(() =>
        {
            return WaitHandle.WaitTimeout != WaitHandle.WaitAny(new[] { Semaphore, ct.WaitHandle });
        }, ct).ConfigureAwait(false);

        ct.ThrowIfCancellationRequested();
        return success;
    }

    public void Release() => Semaphore.Release();

    public void Dispose() => Semaphore.Dispose();
}
