using System;
using System.IO;

namespace InterReact
{
    /// <summary>
    /// Alert can represent an error, warning or informative message.
    /// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
    /// For messages which are not associated with a particular request or order, the Id is -1.
    /// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
    /// </summary>
    public sealed class Alert : IHasRequestId, IHasOrderId
    {
        public int RequestId { get; }
        public int OrderId { get; }
        public int Code { get; }
        public string Message { get; }
        public AlertType AlertType { get; }

        internal Alert(int id, int code, string message)
        {
            RequestId = OrderId = id;
            Code = code;
            Message = message;
            AlertType = GetAlertTypeFromCode(code, id);
        }

        internal static Alert Create(ResponseReader r)
        {
            r.RequireVersion(2);
            return new Alert(r.ReadInt(), r.ReadInt(), r.ReadString());
        }

        internal InvalidOperationException ToException()
        {
            InvalidOperationException exception = new(Message);
            exception.Data.Add("Alert", this);
            return exception;
        }

        public override string ToString()
        {
            string m = Message;
            if (!m.EndsWith("."))
                m += ".";
            if (Code >= 0)
                m += " Code=" + Code + ".";
            if (RequestId >= 0)
                m += $" Id={RequestId}.";
            return m;
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
    }
}
