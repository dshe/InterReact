
using System;
using System.Text.RegularExpressions;

namespace InterReact
{
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
        //public AlertType AlertType { get; } = AlertType.Undefined;

        internal Alert() { }
        internal Alert(int id, int code, string message)
        {
            RequestId = OrderId = id;
            Code = code;
            Message = message;
            IsFatal = code != 10167; // "Requested market data is not subscribed. Displaying delayed market data."
            //AlertType = GetAlertTypeFromCode(code, id);
        }

        internal static Alert Create(ResponseReader r)
        {
            r.RequireVersion(2);
            int id = r.ReadInt();
            int code = r.ReadInt();
            string msg = r.ReadString();
            if (r.Config.SupportsServerVersion(ServerVersion.ENCODE_MSG_ASCII7))
                msg = Regex.Unescape(msg);
            return new Alert(id, code, msg);
        }

        private static AlertType GetAlertTypeFromCode(int code, int id)
        {
            if (code is 1100 or 2110)
                return AlertType.ConnectionLost;
            if (code is 1101 or 1102)
                return AlertType.ConnectionRestored;
            if (code is >= 2103 and <= 2108)
                return AlertType.DataFarm;
            if (id >= 0)
                return AlertType.HasId;
            return AlertType.Undefined;
        }

        internal AlertException ToException() => new(this);
    }

    public sealed class AlertException : Exception
    {
        public Alert Alert { get; }
        public AlertException(Alert alert) : base(alert.Message) => Alert = alert;
    }
}
