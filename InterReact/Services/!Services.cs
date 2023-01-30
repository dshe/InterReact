namespace InterReact;

public partial class Service
{
    private readonly Request Request;
    private readonly IObservable<object> Response;

    public Service(Request request, IObservable<object> response)
    {
        Request = request;
        Response = response;
        AccountUpdatesObservable = CreateAccountUpdatesObservable();
        AccountSummaryObservable = CreateAccountSummaryObservable();
        PositionsObservable = CreatePositionsObservable();
    }
}
