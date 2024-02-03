namespace InterReact;

public partial class Service : IDisposable
{
    private Request Request { get; }
    private Response Response { get; }
    private bool Disposed;

    public Service(Request request, Response response)
    {
        Request = request;
        Response = response;
        AccountSummaryObservable = CreateAccountSummaryObservable();
        PositionsObservable = CreatePositionsObservable();
        ManagedAccountsObservable = CreateManagedAccountsObservable();
    }

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
