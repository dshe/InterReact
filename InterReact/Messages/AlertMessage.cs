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
    public bool IsFatal { get; init; }
    public string Message { get; init; }
    public string AdvancedOrderRejectJson { get; }
    internal Instant Time { get; init; }
    internal AlertMessage() 
    {
        RequestId = OrderId = -1;
        Message = "";
        AdvancedOrderRejectJson = "";
    }
    internal AlertMessage(ResponseReader r, IClock clock)
    {
        r.RequireMessageVersion(2);
        RequestId = OrderId = r.ReadInt();
        Code = r.ReadInt();
        // alerts are fatal by default if they are associated with a request
        IsFatal = Alert.GetAlert(Code)?.IsFatal ?? RequestId > 0; 
        Message = Regex.Unescape(r.ReadString());
        string tempStr = r.ReadString();
        AdvancedOrderRejectJson = string.IsNullOrEmpty(tempStr) ? "" : Regex.Unescape(tempStr);
        Time = clock.GetCurrentInstant();
    }
    public AlertException ToAlertException() => new(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public sealed class AlertException : Exception
{
    public AlertMessage AlertMessage { get; }
    internal AlertException(AlertMessage alert) : base($"Alert: {alert.Message} ({alert.Code})") => AlertMessage = alert;
}
