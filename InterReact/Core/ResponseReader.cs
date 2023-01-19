using Microsoft.Extensions.Logging;
using NodaTime.Text;
using StringEnums;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterReact;

public sealed class ResponseReader
{
    internal InterReactClientConnector Connector { get; }
    internal ResponseParser Parser { get; }
    internal ILogger Logger { get; }
    internal ResponseReader(InterReactClientConnector connector)
    {
        Connector = connector;
        Logger = connector.Logger;
        Parser = new ResponseParser(Logger);
    }

    private IEnumerator<string> Enumerator = Enumerable.Empty<string>().GetEnumerator();
    internal void SetEnumerator(string[] strings) =>
        Enumerator = strings.AsEnumerable().GetEnumerator();

    internal string ReadString()
    {
        if (Enumerator.MoveNext())
            return Enumerator.Current;
        throw new InvalidDataException("Message is shorter than expected.");
    }

    internal void VerifyEnumerationEnd()
    {
        if (Enumerator.MoveNext())
            throw new InvalidDataException("Message longer than expected.");
    }

    internal bool ReadBool() => Parser.ParseBool(ReadString());
    internal char ReadChar() => Parser.ParseChar(ReadString());

    internal int ReadInt() => Parser.ParseInt(ReadString());
    internal int? ReadIntNullable() => Parser.ParseIntNullable(ReadString());

    internal long ReadLong() => Parser.ParseLong(ReadString());

    internal double ReadDouble() => Parser.ParseDouble(ReadString());
    internal double ReadDoubleMax() => Parser.ParseDoubleMax(ReadString());
    internal double? ReadDoubleNullable() => Parser.ParseDoubleNullable(ReadString());

    internal decimal ReadDecimal() => Parser.ParseDecimal(ReadString());

    internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
    internal LocalDateTime ReadLocalDateTime(LocalDateTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
    
    internal T ReadEnum<T>() where T : Enum => Parser.ParseEnum<T>(ReadString());
    internal T ReadStringEnum<T>() where T : StringEnum<T>, new() => Parser.ParseStringEnum<T>(ReadString());

    internal void IgnoreMessageVersion() => ReadString();
    internal int GetMessageVersion() => ReadInt();
    internal void RequireMessageVersion(int minimumVersion)
    {
        int v = GetMessageVersion();
        if (v < minimumVersion)
            throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
    }
    
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
            list.Add(new Tag(ReadString(), ReadString()));
    }
}
