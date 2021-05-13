using System;

namespace InterReact
{
    /// <summary>
    /// Alert can represent an error, warning or informative message.
    /// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
    /// For messages which are not associated with a particular request or order, the Id is -1.
    /// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
    /// They are always set to the same value.
    /// </summary>
    public sealed class Alert : Exception, IHasRequestId, IHasOrderId
    {
        public int Code { get; }
        public int RequestId { get; }
        public int OrderId { get; }
        public AlertType AlertType { get; }

        internal Alert(int id, int code, string message) : base(message)
        {
            RequestId = OrderId = id; // id refers to either a requestId or an orderId.
            Code = code;
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));
            AlertType = GetAlertTypeFromCode(code, id);
        }
        internal static Alert Create(ResponseReader c)
        {
            c.RequireVersion(2);
            return new Alert(c.ReadInt(), c.ReadInt(), c.ReadString());
        }

        public override string ToString()
        {
            var m = Message;
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
            if (code == 1100 || code == 2110)
                return AlertType.ConnectionLost;
            if (code == 1101 || code == 1102)
                return AlertType.ConnectionRestored;
            if (code >= 2103 && code <= 2108)
                return AlertType.DataFarm;
            if (id >= 0)
                return AlertType.HasId;
            return AlertType.Undefined;
        }
    }
}
