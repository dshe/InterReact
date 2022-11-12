using System.Collections.Generic;
using System.Globalization;
using StringEnums;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace InterReact;

internal sealed class ResponseParser
{
    private readonly Dictionary<Type, Dictionary<string, object>> EnumCache = new();
    private readonly ILogger Logger;

    internal ResponseParser(ILogger logger) => Logger = logger;

    internal static char ParseChar(string s)
    {
        if (s.Length == 0)
            return '\0';
        if (s.Length == 1)
            return s[0];
        throw new ArgumentException($"ParseChar>('{s}') failure.");
    }

    internal static bool ParseBool(string s)
    {
        if (s.Length == 0 || s == "0" || string.Equals(s, "false", StringComparison.OrdinalIgnoreCase))
            return false;
        if (s == "1" || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase))
            return true;
        if (int.TryParse(s, out int _))
            return true;
        throw new ArgumentException($"ParseBool('{s}') failure.");
    }

    internal static int ParseInt(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0;
        if (int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out int n))
            return n;
        throw new ArgumentException($"ParseInt('{s}') failure.");
    }

    internal static long ParseLong(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0L;
        if (long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out long n))
            return n;
        throw new ArgumentException($"ParseLong('{s}') failure.");
    }

    internal static double ParseDouble(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0D;
        if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double n))
            return n;
        throw new ArgumentException($"ParseDouble('{s}') failure.");
    }

    internal static int? ParseIntNullable(string s) => (s.Length == 0 || s == "2147483647") ? null : ParseInt(s);
    internal static double? ParseDoubleNullable(string s) => (s.Length == 0 || s == "1.7976931348623157E308") ? null : ParseDouble(s);

    internal T ParseEnum<T>(string numberString) where T : Enum
    {
        Type type = typeof(T);
        if (!EnumCache.TryGetValue(type, out var enumValues))
        {
            enumValues = Enum.GetValues(type).OfType<object>().ToDictionary(x => ((int)x).ToString(CultureInfo.InvariantCulture));
            EnumCache.Add(type, enumValues);
        }
        if (!enumValues.TryGetValue(numberString, out object? e))
        {
            if (!int.TryParse(numberString, out int number))
                throw new ArgumentException($"ParseEnum<{type.Name}>('{numberString}') is not an integer.");
            e = Enum.ToObject(type, number);
            enumValues.Add(numberString, e);
            Logger.LogTrace("ParseEnum<{Name}>('{NumberString}') new value.", type.Name, numberString);
        }
        return (T)e;
    }

    internal T ParseStringEnum<T>(string s) where T : StringEnum<T>, new()
    {
        T? e = StringEnum<T>.ToStringEnum(s);
        if (e is null)
        {
            e = StringEnum<T>.Add(s);
            if (e is null)
                throw new InvalidOperationException($"Could not add new value {s} to StringEnum.");
            Logger.LogTrace("ParseStringEnum<{Name}>('{E}') added new value.", typeof(T).Name, e);
        }
        return e;
    }
}
