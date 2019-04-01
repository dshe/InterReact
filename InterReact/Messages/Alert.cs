using System;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Alert can represent an error, warning or informative message.
    /// The Id, when >= 0, indicates that the message is associated with a particular request(requestId) or order(orderId).
    /// For messages which are not associated with a particular request or order, the Id is -1.
    /// Alert is used to report BOTH error and status messages.
    /// When an error is received from IB which contains an id which is 0 or greater, the id could be either a requestId or an orderId.
    /// Errors which are not associated with a particular request or order have Id = -1 (requestId = orderId = -1).
    /// In order to be compatible with the IHasRequestId and IHasOrderId interfaces, both requestId and orderId properties are included in Alert.
    /// They are always set to the same value.
    /// </summary>
    public sealed class Alert : IHasRequestId, IHasOrderId
    {
        public int Code { get; }
        public int RequestId { get; }
        public int OrderId { get; } 
        public AlertType AlertType { get; }
        public string Message { get; }

        internal Alert(int id, int code, string message)
        {
            Code = code;
            RequestId = OrderId = id; // id refers to either a requestId or an orderId.
            if (id >= 0)
                AlertType = AlertType.HasId;
            else if (code == 1100 || code == 2110)
                AlertType = AlertType.ConnectionLost;
            else if (code == 1101 || code == 1102)
                AlertType = AlertType.ConnectionRestored;
            else if (code >= 2103 && code <= 2108)
                AlertType = AlertType.DataFarm;
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));
            Message = message;
            if (!Message.EndsWith("."))
                Message += ".";
            if (code != -1)
                Message += " Code=" + code + ".";
            if (RequestId != -1)
                Message += $" Id={RequestId}.";
        }

        internal Exception ToAlertException() =>
            new AlertException(this);

        internal static Alert Create(ResponseComposer c)
        {
            c.RequireVersion(2);
            return new Alert(c.ReadInt(), c.ReadInt(), c.ReadString());
        }
    }

    public sealed class AlertException : Exception
    {
        public Alert Alert { get; }

        internal AlertException(Alert alert) : base(alert.Message) => Alert = alert;
    }
}
