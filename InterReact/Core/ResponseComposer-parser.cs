using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using StringEnums;
using System.Linq;
using NodaTime.Text;
using NodaTime;

namespace InterReact.Core
{
    public sealed partial class ResponseComposer
    {
        private readonly static LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
        private readonly static LocalDateTimePattern DateTimePattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd HH:mm:ss");

        internal string ReadString() => 
            enumerator.MoveNext() ? enumerator.Current : throw new IndexOutOfRangeException($"Message shorter than expected.");

        internal T Read<T>() => Parse<T>(ReadString());

        internal T Parse<T>(string s) => (T)ParseImpl<T>(s);

        private object ParseImpl<T>(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var type = typeof(T);

            if (type == typeof(string))
                return s;

            if (type == typeof(LocalTime))
                return TimePattern.Parse(s).Value;

            if (type == typeof(LocalDateTime))
                return DateTimePattern.Parse(s).Value;

            if (type == typeof(bool))
            {
                if (s == "0" || string.Compare(s, "false", StringComparison.OrdinalIgnoreCase) == 0)
                    return false;
                if (s == "1" || string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
                throw new InvalidDataException($"Parse<bool>('{s}') failure.");
            }

            if (type == typeof(char))
            {
                if (s.Length == 1)
                    return s[0];
                throw new InvalidDataException($"Parse<char>('{s}') failure.");
            }

            if (type.GetTypeInfo().IsEnum)
                return ParseEnum(type, s);

            var utype = Nullable.GetUnderlyingType(type);
            if (utype != null)
            {
                if (utype != typeof(int) && utype != typeof(long) && utype != typeof(double))
                    throw new InvalidDataException($"Parse: '{utype.Name}' nullable is not supported.");
                if (s.Length == 0)
                    return null;
                type = utype;
            }

            if (s.Length == 0)
                return 0;

            if (type == typeof(int))
            {
                if (int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out int n))
                    return n;
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out long n))
                    return n;
            }
            else if (type == typeof(double))
            {
                if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double n))
                    return n;
            }

            throw new InvalidDataException($"Parse<{typeof(T).Name}>('{s}') failure.");
        }

        /////////////////////////////////////////////////////////////////////

        private readonly Dictionary<Type, Dictionary<string, object>> enumCache = new Dictionary<Type, Dictionary<string, object>>();

        private object ParseEnum(Type type, string numberString)
        {
            if (!enumCache.TryGetValue(type, out var enumValues))
            {
                enumValues = Enum.GetValues(type).OfType<object>().ToDictionary(x => ((int)(x)).ToString());
                enumCache.Add(type, enumValues);
            }
            if (!enumValues.TryGetValue(numberString, out object e))
            {
                if (!int.TryParse(numberString, out var number))
                    throw new InvalidDataException($"ParseEnum<{type.Name}>('{numberString}') is not an integer.");
                e = Enum.ToObject(type, number);
                enumValues.Add(numberString, e);
                output(new ResponseWarning($"ParseEnum<{type.Name}>('{numberString}') new value."));
            }
            return e;
        }

        /////////////////////////////////////////////////////////////////////

        internal T ReadStringEnum<T>() where T : StringEnum<T>, new() => ParseStringEnum<T>(ReadString());

        internal T ParseStringEnum<T>(string s) where T : StringEnum<T>, new()
        {
            var e = StringEnum<T>.ToStringEnum(s);
            if (e == null)
            {
                e = StringEnum<T>.Add(s);
                output(new ResponseWarning($"ReadStringEnum<{typeof(T).Name}>('{e}') added new value."));
            }
            return e;
        }

        /////////////////////////////////////////////////////////////////////

        private void IgnoreVersion() => ReadString();
        private int GetVersion() => Read<int>();
        private int RequireVersion(int minimumVersion)
        {
            var v = GetVersion();
            return (v >= minimumVersion) ? v : throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
        }
    }
}
