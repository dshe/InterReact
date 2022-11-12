using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterReact;

/// <summary>
/// Alert can represent an error, warning or informative message.
/// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
/// For messages which are not associated with a particular request or order, the Id is -1.
/// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
/// </summary>
public sealed class Alert : IHasRequestId, IHasOrderId, ITick
{
    public int RequestId { get; } = -1;
    public int OrderId { get; } = -1;
    public string Message { get; } = "";
    public int Code { get; }
    public bool IsFatal { get; }
    internal Alert() { }
    internal Alert(int id, int code, string message, bool isFatal)
    {
        RequestId = OrderId = id;
        Code = code;
        Message = message;
        IsFatal = isFatal;
    }

    internal static Alert Create(ResponseReader r)
    {
        r.RequireVersion(2);
        int id = r.ReadInt();
        int code = r.ReadInt();
        string msg = r.ReadString();
        if (r.Connector.SupportsServerVersion(ServerVersion.ENCODE_MSG_ASCII7))
            msg = Regex.Unescape(msg);
        return new Alert(id, code, msg, IsFatalCode(id, code));
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

    internal AlertException ToException() => new(this);
}

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
public sealed class AlertException : Exception
{
    public Alert Alert { get; }
    internal AlertException(Alert alert) : base(alert.Message) => Alert = alert;
}
