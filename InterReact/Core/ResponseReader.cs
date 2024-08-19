using Stringification;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace InterReact;

public sealed class ResponseReader(
    ILogger<ResponseReader> logger, InterReactOptions options, ResponseParser parser)
{
    internal ILogger Logger { get; } = logger;
    internal ResponseParser Parser { get; } = parser;
    internal InterReactOptions Options { get; } = options;
    private string CallerInfo = "";
    internal string[] Strings = [];
    private int Index;

    internal void SetStrings(string[] strings)
    {
        Strings = strings;
        Index = 0;
    }

    private static string GetCallerInfo(string member, string file, int l)
    {
        string line = File.ReadLines(file).Skip(l - 1).First().Trim();
        string projectName = Assembly.GetExecutingAssembly().GetName().Name ?? throw new InvalidOperationException("ProjectName not found");
        int pos = file.LastIndexOf('\\' + projectName + '\\', StringComparison.Ordinal);
        string path = file[(pos + projectName.Length + 2)..];
        return $"{path}:{l} {member}\r\n\t{line}";
    }
    
    internal void VerifyEnumerationEnd()
    {
        if (Index == Strings.Length)
            return;
        string[] extra = Strings.Skip(Index).Select(x => $"'{x}'").ToArray();
        string msg = $"ResponseReader: long message => [{extra.JoinStrings(", ")}]\r\n{CallerInfo}";
        throw new InvalidDataException(msg);
    }
    
    private T Read<T>(Func<string, T> convert, string member, string file, int l)
    {
        if (Index >= Strings.Length)
        {   // find line in source that attempts to retrieve missing data
            throw new InvalidDataException(
                $"ResponseReader: short message => [{Strings.JoinStrings(", ")}]" +
                $"\r\n{GetCallerInfo(member, file, l)}");
        }

        string input = Strings[Index++];
        T output = convert(input);

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            CallerInfo = GetCallerInfo(member, file, l); // slow!
            Logger.LogDebug(
                "{CallerInfo}\r\n\t\"{Input}\" => {Output} ({Type})",
                    CallerInfo, input, output, typeof(T).Name);
        }

        return output;
    }

    internal string ReadString([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(x => x, member, file, line);

    internal bool ReadBool([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseBool, member, file, line);

    internal char ReadChar([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseChar, member, file, line);
    
    internal int ReadInt([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseInt, member, file, line);

    internal int ReadIntMax([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseIntMax, member, file, line);

    internal long ReadLong([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseLong, member, file, line);

    internal double ReadDouble([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseDouble, member, file, line);

    internal double ReadDoubleMax([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseDoubleMax, member, file, line);

    internal decimal ReadDecimal([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseDecimal, member, file, line);

    //internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(Read()).GetValueOrThrow();
    //internal ZonedDateTime ReadZonedDateTime(ZonedDateTimePattern p) => p.Parse(Read()).GetValueOrThrow();

    internal T ReadEnum<T>([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : Enum
    {
        T t = Read(Parser.ParseEnum<T>, member, file, line);
        if (!Options.UseDelayedTicks && t is TickType tickType)
            return (T)(object)tickType.UndelayTick();
        return t;
    }

    internal void IgnoreMessageVersion([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(x => x, member, file, line);

    internal int GetMessageVersion([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(Parser.ParseInt, member, file, line);

    internal void RequireMessageVersion(int minimumVersion, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        int v = Read(Parser.ParseInt, member, file, line);
        if (v < minimumVersion)
            throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
    }

    internal List<string> GetStringList([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        int n = Read(Parser.ParseInt, member, file, line);
        List<string> list = new(n);
        for (int i = 0; i < n; i++)
            list.Add(Read(x => x, member, file, line));
        return list;
    }

    internal void AddToTags(IList<Tag> tags, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        int n = Read(Parser.ParseInt, member, file, line);
        for (int i = 0; i < n; i++)
            tags.Add(new Tag(
                Read(x => x, member, file, line),
                Read(x => x, member, file, line)));
    }
}
