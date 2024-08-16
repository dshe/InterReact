namespace InterReact;

public partial class Service(Request request, Response response) : IDisposable
{
    private readonly Request Request = request;
    private readonly Response Response = response;
    private bool Disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
            return;

        if (disposing)
        {
            // dispose managed objects
        }

        Disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
