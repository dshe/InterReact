using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace InterReact;

[SuppressMessage("Usage", "CA1822", Scope = "member")]
public sealed class ResponseParser(ILogger<ResponseParser> logger)
{
    private const string _maxInt = "2147483647";
    private const string _maxLong = "9223372036854775807";
    private const string _maxDouble = "1.7976931348623157E308";
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

    internal decimal ParseDecimal(string s)
    {
        if (s.Length == 0 || s == _maxInt || s == _maxLong || s == _maxDouble)
            return decimal.MaxValue;
        if (s == "0")
            return 0M;
        if (decimal.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out decimal n))
            return n;
        throw new ArgumentException($"ParseDecimal('{s}') failure.");
    }

    internal T ParseEnum<T>(string numberString) where T:struct, Enum
    {
        if (!long.TryParse(numberString, CultureInfo.InvariantCulture, out long number))
            throw new ArgumentException($"ParseEnum<{typeof(T).Name}>('{numberString}') is not a number.");
        if (!EnumCache<T>.IsDefined(number))
            Logger.LogWarning("ParseEnum<{Name}>('{NumberString}') is unexpected.", typeof(T).Name, numberString);
        return (T)Enum.ToObject(typeof(T), number);
    }

    internal T ParseStringEnum<T>(string codeString) where T:class
    {
        if (StringEnumCache<T>.Values.TryGetValue(codeString, out T? value))
            return value;
        Logger.LogWarning("GetStringEnum<{Type}>({Code}) is unexpected.", typeof(T).Name, codeString);
        return StringEnumCache<T>.Factory(codeString);
    }
}
