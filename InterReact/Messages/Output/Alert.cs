using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterReact;

/// <summary>
/// Alert can represent an error, warning or informative message.
/// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
/// For messages which are not associated with a particular request or order, the Id is -1.
/// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
/// </summary>
public sealed class Alert : IHasRequestId, IHasOrderId
{
    internal int Id { get; init; }
    public int RequestId => Id;
    public int OrderId => Id;
    public string Message { get; init; }
    public int Code { get; init; }
    public bool IsFatal { get; init; }
    public Instant Time { get; init; }
    internal Alert() { Message = ""; }
    internal Alert(ResponseReader r)
    {
        r.RequireMessageVersion(2);
        Id = r.ReadInt();
        Code = r.ReadInt();
        Message = r.ReadString();
        if (r.Connector.SupportsServerVersion(ServerVersion.ENCODE_MSG_ASCII7))
            Message = Regex.Unescape(Message);
        Time = r.Connector.Clock.GetCurrentInstant();
        IsFatal = IsFatalCode(Id, Code);
    }

    // https://interactivebrokers.github.io/tws-api/message_codes.html
    private static bool IsFatalCode(int id, int code)
    {
        // "Requested market data is not subscribed. Displaying delayed market data."
        if (code == 10167)
            return false;

        if (id < 1)
        {
            if (code >= 500 && code <= 599)
                return true;
            if (code == 1300) // port changed
                return true;
            return false;
        }

        return true;
    }

    public AlertException ToException() => new(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
public sealed class AlertException : Exception
{
    public Alert Alert { get; }
    internal AlertException(Alert alert) : base(alert.Message) => Alert = alert;
}
