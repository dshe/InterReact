using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using StringEnums;
using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public class ResponseReader
    {
        private readonly IEnumerator<string> Enumerator;
        internal readonly Config Config;
        internal readonly ResponseParser Parser;

        internal ResponseReader(Config config, ResponseParser responseParser, string[] strings)
        {
            Config = config;
            Parser = responseParser;
            Enumerator = strings.AsEnumerable().GetEnumerator();
        }

        internal void VerifyMessageEnd()
        {
            if (Enumerator.MoveNext())
                throw new IndexOutOfRangeException("Message longer than expected.");
        }

        internal string ReadString()
        {
            if (Enumerator.MoveNext())
                return Enumerator.Current;
            throw new IndexOutOfRangeException("Message is shorter than expected.");
        }

        internal char ReadChar() => Parser.ParseChar(ReadString());
        internal bool ReadBool() => Parser.ParseBool(ReadString());
        internal int ReadInt() => Parser.ParseInt(ReadString());
        internal long ReadLong() => Parser.ParseLong(ReadString());
        internal double ReadDouble() => Parser.ParseDouble(ReadString());
        internal int? ReadIntNullable() => Parser.ParseIntNullable(ReadString());
        internal double? ReadDoubleNullable() => Parser.ParseDoubleNullable(ReadString());
        internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal LocalDateTime ReadLocalDateTime(LocalDateTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal T ReadEnum<T>() where T : Enum => Parser.ParseEnum<T>(ReadString());
        internal T ReadStringEnum<T>() where T : StringEnum<T>, new() => Parser.ParseStringEnum<T>(ReadString());

        internal void IgnoreVersion() => ReadString();
        internal int GetVersion() => ReadInt();
        internal int RequireVersion(int minimumVersion)
        {
            var v = GetVersion();
            return (v >= minimumVersion) ? v : throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
        }
        internal void AddStringsToList(IList<string> list)
        {
            var n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(ReadString());
        }
        internal void AddTagsToList(IList<Tag> list)
        {
            var n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(new Tag(this));
        }
    }
}