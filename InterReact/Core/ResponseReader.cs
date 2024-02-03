using NodaTime.Text;
using System.IO;

namespace InterReact;

public sealed class ResponseReader
{
    internal ILogger Logger { get; }
    internal ResponseParser Parser { get; }
    internal InterReactOptions Options { get; }

    public ResponseReader(ILogger<ResponseReader> logger, InterReactOptions options, ResponseParser parser)
    {
        Logger = logger;
        Options = options;
        Parser = parser;
    }

    private IEnumerator<string> Enumerator = Enumerable.Empty<string>().GetEnumerator();
    internal void SetEnumerator(IEnumerable<string> strings) =>
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
    internal int ReadIntMax() => Parser.ParseIntMax(ReadString());

    internal long ReadLong() => Parser.ParseLong(ReadString());

    internal double ReadDouble() => Parser.ParseDouble(ReadString());
    internal double ReadDoubleMax() => Parser.ParseDoubleMax(ReadString());

    internal decimal ReadDecimal() => Parser.ParseDecimal(ReadString());

    internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();

    internal ZonedDateTime ReadZonedDateTime(ZonedDateTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();

    internal T ReadEnum<T>() where T : Enum
    {
        T t = Parser.ParseEnum<T>(ReadString());
        if (!Options.UseDelayedTicks && t is TickType tickType)
            return (T)(object)UndelayTick(tickType);
        return t;
    }

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
            list.Add(new Tag(this));
    }

    private static TickType UndelayTick(TickType tickType)
    {
        return tickType switch
        {
            TickType.DelayedBidPrice => TickType.BidPrice,
            TickType.DelayedAskPrice => TickType.AskPrice,
            TickType.DelayedLastPrice => TickType.LastPrice,
            TickType.DelayedBidSize => TickType.BidSize,
            TickType.DelayedAskSize => TickType.AskSize,
            TickType.DelayedLastSize => TickType.LastSize,
            TickType.DelayedHighPrice => TickType.HighPrice,
            TickType.DelayedLowPrice => TickType.LowPrice,
            TickType.DelayedVolume => TickType.Volume,
            TickType.DelayedClosePrice => TickType.ClosePrice,
            TickType.DelayedOpenPrice => TickType.OpenPrice,
            TickType.DelayedBidOptionComputation => TickType.BidOptionComputation,
            TickType.DelayedAskOptionComputation => TickType.AskOptionComputation,
            TickType.DelayedLastOptionComputation => TickType.LastOptionComputation,
            TickType.DelayedModelOptionComputation => TickType.ModelOptionComputation,
            TickType.DelayedLastTimeStamp => TickType.LastTimeStamp,
            _ => tickType
        };
    }
}
