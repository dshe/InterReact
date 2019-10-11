using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using InterReact.Messages;
using StringEnums;
using InterReact.Extensions;
using RxSockets;
using InterReact.Utility;

#nullable enable

namespace InterReact.Core
{
    public sealed class RequestMessage
    {
        private readonly List<string> strings = new List<string>();
        private readonly Action<byte[], int, int> sendAction;
        private readonly Limiter limiter;

        public RequestMessage(IRxSocketClient rxSocket, Limiter limiter) : this(rxSocket.Send, limiter) { }
        internal RequestMessage(Action<byte[], int, int> sendAction, Limiter limiter)
        {
            this.sendAction = sendAction;
            this.limiter    = limiter;
        }

        // V100Plus format: 4 byte message length prefix plus payload of null-terminated strings.
        internal void Send()
        {
            var bytes = strings.ToByteArrayWithLengthPrefix();
            limiter.Limit(() => sendAction(bytes, 0, bytes.Length));
        }

        /////////////////////////////////////////////////////

        internal RequestMessage Write(params object?[]? objs)
        {
            if (objs == null)
                strings.Add(string.Empty);
            else if (objs.Length == 0)
                throw new ArgumentException(nameof(objs));
            else foreach (var o in objs)
                strings.Add(GetString(o));
            return this;
        }

        private string GetString(object? o)
        {
            if (o == null)
                return "";

            switch (o)
            {
                case string s:
                    return s;
                case bool bo:
                    return bo ? "1" : "0";
                case Enum e:
                    var ut = Enum.GetUnderlyingType(e.GetType());
                    return Convert.ChangeType(e, ut).ToString();
            }

            var type = o.GetType();

            if (type.IsStringEnum())
                return o.ToString();

            var utype = Nullable.GetUnderlyingType(type);
            if (utype != null)
            {
                if (utype != typeof(int) && utype != typeof(long) && utype != typeof(double))
                    throw new InvalidDataException($"Nullable '{utype.Name}' is not supported.");
                o = Convert.ChangeType(o, utype);
            }

            switch (o)
            {
                case int i:
                    return i.ToString(NumberFormatInfo.InvariantInfo);
                case long l:
                    return l.ToString(NumberFormatInfo.InvariantInfo);
                case double d:
                    return d.ToString(NumberFormatInfo.InvariantInfo);
            }

            throw new InvalidDataException($"RequestMessage: unsupported data type = {o.GetType().Name}.");
        }

        internal RequestMessage WriteContract(Contract c) =>
            Write(c.ContractId, c.Symbol, c.SecurityType, c.LastTradeDateOrContractMonth, c.Strike, c.Right, c.Multiplier, 
                c.Exchange, c.PrimaryExchange, c.Currency, c.LocalSymbol, c.TradingClass, c.IncludeExpired);
    }

}
