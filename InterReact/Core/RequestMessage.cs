using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
namespace InterReact;

public sealed class RequestMessage(Connection connection, ILogger<Request> logger)
{
    internal List<string> Strings { get; } = []; // internal for testing

    internal async ValueTask SendAsync([CallerMemberName] string memberName = "")
    {
        logger.LogDebug("{Method}({Strings}]).", memberName, Strings.JoinStrings(", "));
        if (Strings.Count == 0)
            throw new InvalidOperationException("Empty send message.");
        await connection.SendMessageAsync(Strings).ConfigureAwait(false);
        Strings.Clear(); // allows the message to be reused
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
            return [string.Empty];
        if (objs.Length == 0)
            throw new ArgumentException("Invalid length", nameof(objs));
        return objs.Select(GetString);
    }

    private static string GetString(object? o)
    {
        switch (o)
        {
            case null:             return "";
            case string s:         return s;
            case bool b:           return b ? "1" : "0";
            case int i:            return i.ToString(CultureInfo.InvariantCulture);
            case long l:           return l.ToString(CultureInfo.InvariantCulture);
            case double d:         return d.ToString(CultureInfo.InvariantCulture);
            case decimal m:        return m == decimal.MaxValue ? "" : m.ToString(CultureInfo.InvariantCulture);
            case IHasStringCode hasCode: return hasCode.StringCode;
        }

        if (o is Enum e)
        {
            Type ut = Enum.GetUnderlyingType(o.GetType());
            return Convert.ChangeType(e, ut, CultureInfo.InvariantCulture)!.ToString()!;
        }

        throw new InvalidDataException($"Cannot serialize type = '{o.GetType().FullName}'.");
    }

    internal RequestMessage WriteEnumValuesString<T>(IEnumerable<T>? enums) where T : Enum
    {
        if (enums is null)
            return Write("");
        return Write(string.Join(",", enums.Select(GetEnumValueString)));

        static string GetEnumValueString(T en)
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));
            object o = Convert.ChangeType(en, underlyingType, CultureInfo.InvariantCulture)
                ?? throw new ArgumentException("Could not get enum value.");
            return o.ToString() ?? throw new ArgumentException("Could not get enum value.");
        }
    }

    internal RequestMessage WriteContract(Contract contract, bool includePrimaryExchange = true) =>
        contract.Write(this, includePrimaryExchange);
}

