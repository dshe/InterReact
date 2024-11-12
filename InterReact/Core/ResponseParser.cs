using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace InterReact;

[SuppressMessage("Usage", "CA1822", Scope = "member")]
public sealed class ResponseParser(ILogger<ResponseParser> logger)
{
    private const string MaxInt = "2147483647";
    private const string MaxLong = "9223372036854775807";
    private const string MaxDouble = "1.7976931348623157E308";
    private Dictionary<Type, Dictionary<string, object>> EnumCache { get; } = [];
    private ILogger Logger { get; } = logger;

    internal char ParseChar(string s)
    {
        if (s.Length == 0)
            return '\0';
        if (s.Length == 1)
            return s[0];
        throw new ArgumentException($"ParseChar('{s}') failure.");
    }

    internal bool ParseBool(string s)
    {
        if (s.Length == 0 || s == "0" || string.Equals(s, "false", StringComparison.OrdinalIgnoreCase))
            return false;
        if (s == "1" || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase))
            return true;
        if (int.TryParse(s, out int _))
            return true;
        throw new ArgumentException($"ParseBool('{s}') failure.");
    }

    internal int ParseInt(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0;
        if (int.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out int n))
            return n;
        throw new ArgumentException($"ParseInt('{s}') failure.");
    }
    internal int ParseIntMax(string s) => s.Length == 0 ? int.MaxValue : ParseInt(s);

    internal long ParseLong(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0L;
        if (long.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out long n))
            return n;
        throw new ArgumentException($"ParseLong('{s}') failure.");
    }

    internal double ParseDouble(string s)
    {
        if (s.Length == 0 || s == "0")
            return 0D;
        if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double n))
            return n;
        throw new ArgumentException($"ParseDouble('{s}') failure.");
    }
    internal double ParseDoubleMax(string s)
    {
        if (s.Length == 0)
            return double.MaxValue;
        if (s == "Infinity")
            return double.PositiveInfinity;
        return ParseDouble(s);
    }

    internal decimal ParseDecimal(string s) // ok
    {
        if (s.Length == 0 || s == MaxInt || s == MaxLong || s == MaxDouble)
            return decimal.MaxValue;
        if (s == "0")
            return 0M;
        if (decimal.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out decimal n))
            return n;
        throw new ArgumentException($"ParseDecimal('{s}') failure.");
    }

    internal T ParseEnum<T>(string numberString) where T : Enum
    {
        Type type = typeof(T);
        if (!EnumCache.TryGetValue(type, out Dictionary<string, object>? enumValues))
        {
            enumValues = Enum
                .GetValues(type)
                .OfType<object>()
                .ToDictionary(x => ((int)x).ToString(CultureInfo.InvariantCulture));
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
}
