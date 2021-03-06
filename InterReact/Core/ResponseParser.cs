﻿using System.Collections.Generic;
using System;
using System.Globalization;
using StringEnums;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace InterReact
{
    internal sealed class ResponseParser
    {
        private readonly Dictionary<Type, Dictionary<string, object>> enumCache = new();
        private readonly ILogger Logger;

        internal ResponseParser(ILogger logger) => Logger = logger;

        internal static char ParseChar(string s)
        {
            if (s == "")
                return '\0';
            if (s.Length == 1)
                return s[0];
            throw new ArgumentException($"ParseChar>('{s}') failure.");
        }

        internal static bool ParseBool(string s)
        {
            if (s == "" || s == "0" || string.Compare(s, "false", StringComparison.OrdinalIgnoreCase) == 0)
                return false;
            if (s == "1" || string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            if (int.TryParse(s, out int _))
                return true;
            throw new ArgumentException($"ParseBool('{s}') failure.");
        }

        internal static int ParseInt(string s)
        {
            if (s == "" || s == "0")
                return 0;
            if (int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out int n))
                return n;
            throw new ArgumentException($"ParseInt('{s}') failure.");
        }

        internal static long ParseLong(string s)
        {
            if (s == "" || s == "0")
                return 0L;
            if (long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out long n))
                return n;
            throw new ArgumentException($"ParseLong('{s}') failure.");
        }

        internal static double ParseDouble(string s)
        {
            if (s == "" || s == "0")
                return 0D;
            if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double n))
                return n;
            throw new ArgumentException($"ParseDouble('{s}') failure.");
        }

        internal static int? ParseIntNullable(string s)
        {
            if (s == "")
                return null; // !
            return ParseInt(s);
        }

        internal static double? ParseDoubleNullable(string s)
        {
            if (s == "")
                return null; // !
            return ParseDouble(s);
        }

        internal T ParseEnum<T>(string numberString) where T : Enum
        {
            Type type = typeof(T);
            if (!enumCache.TryGetValue(type, out var enumValues))
            {
                enumValues = Enum.GetValues(type).OfType<object>().ToDictionary(x => ((int)(x)).ToString());
                enumCache.Add(type, enumValues);
            }
            if (!enumValues.TryGetValue(numberString, out object? e))
            {
                if (!int.TryParse(numberString, out int number))
                    throw new ArgumentException($"ParseEnum<{type.Name}>('{numberString}') is not an integer.");
                e = Enum.ToObject(type, number);
                enumValues.Add(numberString, e);
                Logger.LogTrace($"ParseEnum<{type.Name}>('{numberString}') new value.");
            }
            return (T)e;
        }

        internal T ParseStringEnum<T>(string s) where T : StringEnum<T>, new()
        {
            T? e = StringEnum<T>.ToStringEnum(s);
            if (e == null)
            {
                e = StringEnum<T>.Add(s);
                if (e == null)
                    throw new Exception($"Could not add new value {s} to StringEnum.");
                Logger.LogTrace($"ParseStringEnum<{typeof(T).Name}>('{e}') added new value.");
            }
            return e;
        }
    }
}
