using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using StringEnums;
using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public sealed class ResponseReader
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

        internal void IgnoreVersion() => ReadString();
        internal int GetVersion() => ReadInt();
        internal int RequireVersion(int minimumVersion)
        {
            int v = GetVersion();
            if (v < minimumVersion)
                throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
            return v;
        }

        internal bool ReadBool() => ResponseParser.ParseBool(ReadString());
        internal char ReadChar() => ResponseParser.ParseChar(ReadString());
        internal int ReadInt() => ResponseParser.ParseInt(ReadString());
        internal long ReadLong() => ResponseParser.ParseLong(ReadString());
        internal double ReadDouble() => ResponseParser.ParseDouble(ReadString());
        internal int? ReadIntNullable() => ResponseParser.ParseIntNullable(ReadString());
        internal double? ReadDoubleNullable() => ResponseParser.ParseDoubleNullable(ReadString());
        internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal LocalDateTime ReadLocalDateTime(LocalDateTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal T ReadEnum<T>() where T : Enum => Parser.ParseEnum<T>(ReadString());
        internal T ReadStringEnum<T>() where T : StringEnum<T>, new() => Parser.ParseStringEnum<T>(ReadString());

        internal void AddStringsToList(IList<string> list)
        {
            int n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(ReadString());
        }

        internal void AddTagsToList(IList<Tag> list)
        {
            int n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(new Tag(this));
        }
    }
}