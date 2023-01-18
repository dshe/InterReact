namespace InterReact;

public sealed partial class Service
{
    private readonly Request Request;
    private readonly IObservable<object> Response;

    public Service(Request request, IObservable<object> response)
    {
        Request = request;
        Response = response;
        CurrentTimeObservable = CreateCurrentTimeObservable();
        AccountUpdatesObservable = CreateAccountUpdatesObservable();
        PositionsObservable = CreatePositionsObservable();
        AccountSummaryObservable = CreateAccountSummaryObservable();
        NextOrderIdObservable = CreateNextOrderIdObservable();
        OrderStatusReportObservable = CreateOrderStatusReportObservable();
    }
}
