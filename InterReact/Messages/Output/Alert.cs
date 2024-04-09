using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterReact;

/// <summary>
/// Alert can represent an error, warning or informative message.
/// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
/// For messages which are not associated with a particular request or order, the Id is -1.
/// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
/// </summary>
public sealed class AlertMessage : IHasRequestId, IHasOrderId
{
    public int RequestId { get; init; } = -1;
    public int OrderId { get; init; } = -1;
    public int Code { get; init; }
    public string Message { get; init; } = "";
    public string AdvancedOrderRejectJson { get; } = "";
    public bool IsFatal { get; init; }
    internal Instant Time { get; init; }
    internal AlertMessage() { }
    internal AlertMessage(ResponseReader r, IClock clock)
    {
        r.RequireMessageVersion(2);
        RequestId = OrderId = r.ReadInt();
        Code = r.ReadInt();
        Message = Regex.Unescape(r.ReadString());
        AdvancedOrderRejectJson = Regex.Unescape(r.ReadString());
        Time = clock.GetCurrentInstant();
        IsFatal = IsFatalCode(RequestId, Code);
    }

    // https://interactivebrokers.github.io/tws-api/message_codes.html
    private static bool IsFatalCode(int id, int code)
    {
        if (id < 1)
        {
            if (code >= 500 && code <= 599)
                return true;
            if (code == 1300) // port changed
                return true;
            return false;
        }

        // RequestHistoricalData:
        // "Historical Market Data Service error message:API historical data query cancelled: 1"
        if (code == 162)
            return false;

        // "Requested market data is not subscribed. Displaying delayed market data."
        if (code == 10167)
            return false;

        return true;
    }

    public AlertException ToAlertException() => AlertException.Create(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
public sealed class AlertException : Exception
{
    public AlertMessage AlertMessage { get; }
    private AlertException(AlertMessage alert) : base(alert.Message) => AlertMessage = alert;
    public static AlertException Create(AlertMessage alert) => new(alert);
}

public static partial class Extension
{
    // This operator ignore Alerts (messages and errors) with the specified code.
    public static IObservable<T> IgnoreAlertMessage<T>(this IObservable<T> source, int code) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                if (m is AlertMessage alertMessage && alertMessage.Code == code)
                    return; // ignore
                o.OnNext(m);
            },
            o.OnError,
            o.OnCompleted
        ));

    // This operator converts Observable.OnNext(AlertMessage) to an OnError(AlertException).
    public static IObservable<T> AlertMessageToError<T>(this IObservable<T> source) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                if (m is AlertMessage alertMessage) // && (alertMessage.IsFatal || !fatalOnly))
                    o.OnError(alertMessage.ToAlertException());
                else
                    o.OnNext(m);
            },
            o.OnError,
            o.OnCompleted
        ));
}
