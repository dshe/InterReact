namespace InterReact;

public partial class Service : IDisposable
{
    private readonly InterReactOptions _options;
    private readonly Request _request;
    private readonly IObservable<object> _response;
    private bool _disposed;

    public Service(InterReactOptions options, Request request, Response response)
    {
        _options = options;
        _request = request;
        _response = response;
        AccountPositionsObservable = CreateAccountPositionsObservable();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // dispose managed objects
            _managedAccountsSemaphore.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
