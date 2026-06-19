using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
namespace InterReact;

public sealed class RequestMessage
{
    private ILogger Logger { get; }
    private Connection Connection { get; }
    internal List<string> Strings { get; } = []; // internal for testing
    public RequestMessage(ILogger<RequestMessage> logger, Connection connection)
    {
        ArgumentNullException.ThrowIfNull(logger);
        Logger = logger;
        Connection = connection;
    }

    internal async ValueTask SendAsync([CallerMemberName] string memberName = "")
    {
        Logger.LogDebug("Request: {Method}", memberName);
        if (Strings.Count == 0)
            throw new InvalidOperationException("Empty send message.");
        await Connection.SendMessageAsync(Strings).ConfigureAwait(false);
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
            case null: // includes nullable
                return "";
            case string s:
                return s;
            case bool b:
                return b ? "1" : "0";
        }

        Type type = o.GetType();

        if (o is Enum e)
        {
            Type ut = Enum.GetUnderlyingType(type);
            return Convert.ChangeType(e, ut, CultureInfo.InvariantCulture).ToString() ?? "";
        }

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
            decimal m =>
                m is decimal.MaxValue ? "" : m.ToString(NumberFormatInfo.InvariantInfo),
            _ => throw new InvalidDataException($"RequestMessage: unsupported data type = {o.GetType().Name}."),
        };
    }

    internal RequestMessage WriteEnumValuesString<T>(IEnumerable<T>? enums) where T : Enum
    {
        if (enums is null)
            return Write("");
        return Write(string.Join(",", enums.Select(GetEnumValueString)));

        // local
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

