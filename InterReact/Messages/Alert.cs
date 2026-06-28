using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace InterReact;

/// <summary>
/// Alert can represent an error, warning or informative message.
/// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
/// For messages which are not associated with a particular request or order, the Id is -1.
/// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
/// </summary>
[Message]
public sealed record Alert : IHasRequestId, IHasOrderId
{
    public int RequestId { get; init; } = -1;
    public int OrderId { get; init; } = -1;
    public int Code { get; init; }
    public string Message { get; init; } = "";
    public string AdvancedOrderRejectJson { get; init; } = "";
    internal Alert() { }
    internal Alert(ResponseReader r)
    {
        r.RequireMessageVersion(2);
        RequestId = OrderId = r.ReadInt();
        Code = r.ReadInt();
        Message = Regex.Unescape(r.ReadString());
        string tempStr = r.ReadString();
        AdvancedOrderRejectJson = string.IsNullOrEmpty(tempStr) ? "" : Regex.Unescape(tempStr);
    }
    public AlertException ToAlertException() => new(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public sealed class AlertException : Exception
{
    public Alert AlertMessage { get; }
    internal AlertException(Alert alert) : base($"Alert: {alert.Message} ({alert.Code})") => AlertMessage = alert;
}
