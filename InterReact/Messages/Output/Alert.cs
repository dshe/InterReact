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
    public int RequestId { get; init; }
    public int OrderId { get; init; }
    public int Code { get; init; }
    public string Message { get; init; }
    public string AdvancedOrderRejectJson { get; }
    internal Instant Time { get; init; }
    internal AlertMessage() 
    {
        RequestId = OrderId = -1;
        Message = "";
        AdvancedOrderRejectJson = "";
        Time = Instant.MinValue;
    }
    internal AlertMessage(ResponseReader r, IClock clock)
    {
        r.RequireMessageVersion(2);
        RequestId = OrderId = r.ReadInt();
        Code = r.ReadInt();
        Message = Regex.Unescape(r.ReadString());
        AdvancedOrderRejectJson = Regex.Unescape(r.ReadString());
        Time = clock.GetCurrentInstant();
    }
    public AlertException ToAlertException() => new(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
public sealed class AlertException : Exception
{
    internal AlertException(AlertMessage alert) : base(alert.Message + "(" + alert.Code + ")") { }
}

public static partial class Extension
{
    // This operator ignores Alerts (messages and errors) with the specified code.
    public static IObservable<T> IgnoreAlertMessage<T>(this IObservable<T> source, ErrorResponse? error = null) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                if (m is AlertMessage alert && (error == null || alert.Code == error.Code))
                    return; // ignore
                o.OnNext(m);
            },
            o.OnError,
            o.OnCompleted
        ));

    public static IEnumerable<T> IgnoreAlertMessage<T>(this IEnumerable<T> source, ErrorResponse? error = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        foreach (T m in source)
        {
            if (m is AlertMessage alert && (error == null || alert.Code == error.Code))
                continue; // ignore
            yield return m;
        }
    }

    public static IObservable<T> ThrowAlertMessage<T>(this IObservable<T> source, ErrorResponse? error = null) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                if (m is AlertMessage alert && (error == null || alert.Code == error.Code))
                    o.OnError(alert.ToAlertException());
                else
                    o.OnNext(m);
            },
            o.OnError,
            o.OnCompleted
        ));

    public static IObservable<T> CastTo<T>(this IObservable<object> source) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                if (m is T t)
                    o.OnNext(t);
                else if (m is AlertMessage alertMessage)
                    o.OnError(alertMessage.ToAlertException());
                else
                    o.OnError(new InvalidOperationException("Invalid type: " + m.GetType().Name));
            },
            o.OnError,
            o.OnCompleted
        ));
}
