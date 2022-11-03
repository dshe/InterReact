using System.Collections.Generic;
using System.Linq;
using System.IO;
using StringEnums;
using NodaTime;
using NodaTime.Text;
namespace InterReact;

public sealed class ResponseReader
{
    internal InterReactClientConnector Connector { get; }
    private readonly ResponseParser Parser;
    internal ResponseReader(InterReactClientConnector connector)
    {
        Connector = connector;
        Parser = new ResponseParser(connector.Logger);
    }

    private IEnumerator<string>? Enumerator;
    internal void SetEnumerator(string[] strings) => Enumerator = strings.AsEnumerable().GetEnumerator();

    internal string ReadString()
    {
        ArgumentNullException.ThrowIfNull(Enumerator);
        if (Enumerator.MoveNext())
            return Enumerator.Current;
        throw new InvalidDataException("Message is shorter than expected.");
    }

    internal void VerifyEnumerationEnd()
    {
        ArgumentNullException.ThrowIfNull(Enumerator);
        if (Enumerator.MoveNext())
            throw new InvalidDataException("Message longer than expected.");
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
    internal int GetVersion() => ReadInt();
    internal int RequireVersion(int minimumVersion)
    {
        int v = GetVersion();
        if (v < minimumVersion)
            throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
        return v;
    }
    internal void IgnoreVersion() => ReadString();

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
