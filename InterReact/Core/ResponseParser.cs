using System.Collections.Generic;
using System;
using System.Globalization;
using StringEnums;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace InterReact.Core
{
    internal sealed class ResponseParser
    {
        private readonly Dictionary<Type, Dictionary<string, object>> enumCache = new Dictionary<Type, Dictionary<string, object>>();
        private readonly ILogger Logger;

        internal ResponseParser(ILogger logger) => Logger = logger;

        internal char ParseChar(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "")
                return '\0';
            if (s.Length == 1)
                return s[0];
            throw new ArgumentException($"ParseChar>('{s}') failure.");
        }

        internal bool ParseBool(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "" || s == "0" || string.Compare(s, "false", StringComparison.OrdinalIgnoreCase) == 0)
                 return false;
             if (s == "1" || string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            if (int.TryParse(s, out int _))
                return true;
             throw new ArgumentException($"ParseBool('{s}') failure.");
        }

        internal int ParseInt(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "" || s == "0")
                return 0;
            if (int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out int n))
                return n;
            throw new ArgumentException($"ParseInt('{s}') failure.");
        }

        internal long ParseLong(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "" || s == "0")
                return 0L;
            if (long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out long n))
                return n;
            throw new ArgumentException($"ParseLong('{s}') failure.");
        }

        internal double ParseDouble(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "" || s == "0")
                return 0D;
            if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double n))
                return n;
            throw new ArgumentException($"ParseDouble('{s}') failure.");
        }

        internal int? ParseIntNullable(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "")
                return null; // !
            return ParseInt(s);
        }

        internal double? ParseDoubleNullable(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (s == "")
                return null; // !
            return ParseDouble(s);
        }

        internal T ParseEnum<T>(string numberString) where T: Enum
        {
            if (numberString == null)
                throw new ArgumentNullException(nameof(numberString));

            var type = typeof(T);
            if (!enumCache.TryGetValue(type, out var enumValues))
            {
                enumValues = Enum.GetValues(type).OfType<object>().ToDictionary(x => ((int)(x)).ToString());
                enumCache.Add(type, enumValues);
            }
            if (!enumValues.TryGetValue(numberString, out object e))
            {
                if (!int.TryParse(numberString, out var number))
                    throw new ArgumentException($"ParseEnum<{type.Name}>('{numberString}') is not an integer.");
                e = Enum.ToObject(type, number);
                enumValues.Add(numberString, e);
                Logger.LogDebug($"ParseEnum<{type.Name}>('{numberString}') new value.");
            }
            return (T)e;
        }

        internal T ParseStringEnum<T>(string s) where T : StringEnum<T>, new()
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            var e = StringEnum<T>.ToStringEnum(s);
            if (e == null)
            {
                e = StringEnum<T>.Add(s);
                if (e == null)
                    throw new Exception($"Could not add new value {s} to StringEnum.");
                Logger.LogDebug($"ParseStringEnum<{typeof(T).Name}>('{e}') added new value.");
            }
            return e;
        }
    }
}
