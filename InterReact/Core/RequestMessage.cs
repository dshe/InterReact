using System.Collections.Generic;
using System.Globalization;
using System.IO;
using StringEnums;
using System.Linq;

namespace InterReact;

public sealed class RequestMessage
{
    private readonly List<string> Strings = new();
    private readonly Action<List<string>> SendAction;
    internal RequestMessage(Action<List<string>> sendAction) => SendAction = sendAction;

    // V100Plus format: 4 byte message length prefix plus payload of null-terminated strings.
    internal void Send()
    {
        if (!Strings.Any())
            throw new InvalidOperationException("Empty send message.");
        SendAction(Strings);
        Strings.Clear();
    }

    /////////////////////////////////////////////////////

    internal RequestMessage Write(params object?[]? objs)
    {
        Strings.AddRange(GetStrings(objs));
        return this;
    }

    private static IEnumerable<string> GetStrings(object?[]? objs)
    {
        if (objs is null)
            return new[] { string.Empty };
        if (objs.Length == 0)
            throw new ArgumentException("invalid length", nameof(objs));
        return objs.Select(o => GetString(o));
    }

    private static string GetString(object? o)
    {
        if (o is null)
            return "";

        switch (o)
        {
            case string s:
                return s;
            case bool b:
                return b ? "1" : "0";
            case Enum e:
                Type ut = Enum.GetUnderlyingType(e.GetType());
                return Convert.ChangeType(e, ut, CultureInfo.InvariantCulture).ToString() ?? "";
        }

        Type type = o.GetType();

        if (type.IsStringEnum())
            return o.ToString() ?? "";

        Type? utype = Nullable.GetUnderlyingType(type);
        if (utype is not null)
        {
            if (utype != typeof(int) && utype != typeof(long) && utype != typeof(double))
                throw new InvalidDataException($"Nullable '{utype.Name}' is not supported.");
            o = Convert.ChangeType(o, utype, CultureInfo.InvariantCulture);
        }

        return o switch
        {
            bool b => b ? "1" : "0",
            int i => i.ToString(NumberFormatInfo.InvariantInfo),
            long l => l.ToString(NumberFormatInfo.InvariantInfo),
            double d => d.ToString(NumberFormatInfo.InvariantInfo),
            _ => throw new InvalidDataException($"RequestMessage: unsupported data type = {o.GetType().Name}."),
        };
    }

    internal RequestMessage WriteContract(Contract c) =>
        Write(c.ContractId, c.Symbol, c.SecurityType, c.LastTradeDateOrContractMonth, c.Strike, c.Right, c.Multiplier,
            c.Exchange, c.PrimaryExchange, c.Currency, c.LocalSymbol, c.TradingClass, c.IncludeExpired);
}
