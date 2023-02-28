namespace InterReact;

public partial class Service : IDisposable
{
    private readonly AsyncSemaphore MatchingSymbolsSemaphore = new("MatchingSymbolsSemaphore");
    private readonly Request Request;
    private readonly Response Response;
    private bool Disposed;

    public Service(Request request, Response response)
    {
        Request = request;
        Response = response;
        AccountUpdatesObservable = CreateAccountUpdatesObservable();
        AccountSummaryObservable = CreateAccountSummaryObservable();
        PositionsObservable = CreatePositionsObservable();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
            return;

        if (disposing)
        {
            // dispose managed objects
            MatchingSymbolsSemaphore.Dispose();
        }

        Disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
