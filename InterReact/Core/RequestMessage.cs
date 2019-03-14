using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using InterReact.Messages;
using StringEnums;
using InterReact.Extensions;
using RxSockets;
using MinimalContainer;
using InterReact.Utility;

namespace InterReact.Core
{
    internal sealed class RequestMessage
    {
        private readonly List<string> strings = new List<string>();
        private readonly Action<byte[], int, int> sendAction;
        private readonly Limiter limiter;

        [ContainerConstructor]
        internal RequestMessage(IRxSocketClient rxSocket, Limiter limiter) : this(rxSocket.Send, limiter) { }
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
                WriteImpl(o);
            return this;
        }

        private void WriteImpl(object? o)
        {
            if (o == null)
            {
                strings.Add(string.Empty);
                return;
            }

            switch (o)
            {
                case string s:
                    strings.Add(s);
                    return;
                case bool bo:
                    strings.Add(bo ? "1" : "0");
                    return;
                case Enum e:
                    var ut = Enum.GetUnderlyingType(e.GetType());
                    strings.Add(Convert.ChangeType(e, ut).ToString());
                    return;
                default:
                    break;
            }

            var type = o.GetType();

            if (type.IsStringEnum())
            {
                strings.Add(o.ToString());
                return;
            }

            var utype = Nullable.GetUnderlyingType(type);
            if (utype != null)
            {
                if (utype != typeof(int) && utype != typeof(long) && utype != typeof(double))
                    throw new InvalidDataException($"Nullable '{utype.Name}' is not supported.");
                o = Convert.ChangeType(o, utype);
            }

            switch (o) // supported nullable types
            {
                case int i:
                    strings.Add(i.ToString(NumberFormatInfo.InvariantInfo));
                    return;
                case long l:
                    strings.Add(l.ToString(NumberFormatInfo.InvariantInfo));
                    return;
                case double d:
                    strings.Add(d.ToString(NumberFormatInfo.InvariantInfo));
                    return;
            }

            throw new InvalidDataException($"RequestMessage: unsupported data type = {o.GetType().Name}.");
        }

        internal RequestMessage WriteTagsAsOneString(IEnumerable<Tag> tags)
        {
            if (tags != null)
                strings.Add(tags.Select(tag => $"{tag.Name}={tag.Value}").JoinStrings(";"));
            else
                strings.Add(string.Empty);
            return this;
        }

        internal RequestMessage WriteContract(Contract c) =>
            Write(c.ContractId, c.Symbol, c.SecurityType, c.LastTradeDateOrContractMonth, c.Strike, c.Right, c.Multiplier, 
                c.Exchange, c.PrimaryExchange, c.Currency, c.LocalSymbol, c.TradingClass, c.IncludeExpired);
    }

}
