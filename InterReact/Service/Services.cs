namespace InterReact;

public sealed partial class Services
{
    private readonly Request Request;
    private readonly IObservable<object> Response;

    public Services(Request request, IObservable<object> response)
    {
        Request = request;
        Response = response;
        CurrentTimeObservable = CreateCurrentTimeObservable();
        AccountUpdatesObservable = CreateAccountUpdatesObservable();
        PositionsObservable = CreatePositionsObservable();
        AccountSummaryObservable = CreateAccountSummaryObservable();
    }
}
