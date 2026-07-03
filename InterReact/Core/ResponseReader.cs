using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace InterReact;

public sealed class ResponseReader(InterReactOptions options, ResponseParser parser, ILogger<ResponseReader> logger)
{
    private int _index;
    private string[] _strings = [];
    internal ILogger Logger { get; } = logger;
    internal ResponseParser Parser { get; } = parser;
    internal InterReactOptions Options { get; } = options;

    internal void SetStrings(string[] strings)
    {
        _strings = strings;
        _index = 0;
    }

    internal void VerifyEnumerationEnd()
    {
        if (_index == _strings.Length)
            return;
        string[] extra = [.. _strings.Skip(_index).Select(x => $"'{x}'")];
        string msg = $"ResponseReader: long message => [{extra.JoinStrings(", ")}]";
        throw new InvalidDataException(msg);
    }
    
    private T Read<T>(Func<string, T> convert, string member, string file, int l)
    {
        if (_index >= _strings.Length)
            throw new InvalidDataException($"ResponseReader: short message {GetCallerInfo(member, file, l)}");

        string input = _strings[_index++];
        try
        {
            return convert(input);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"ResponseReader: {GetCallerInfo(member, file, l)} {ex.Message}", ex);
        }
    }

    internal string ReadString([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        Read(static x => x, member, file, line);

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

    internal T ReadEnum<T>([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T:struct,Enum
    {
        T t = Read(Parser.ParseEnum<T>, member, file, line);
        if (!Options.UseDelayedTicks && t is TickType tickType)
            return (T)(object)tickType.UndelayTick();
        return t;
    }

    internal T ReadStringEnum<T>([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T:class =>
        Read(Parser.ParseStringEnum<T>, member, file, line);

    internal void IgnoreMessageVersion([CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
        ReadString(member, file, line);

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

    private string GetCallerInfo(string member, string file, int line)
    {
        var inn = $"{_strings.Stringify(false)}";
        string lineText = File.ReadLines(file).Skip(line - 1).First().Trim();
        string projectName = Assembly.GetExecutingAssembly().GetName().Name!;
        int pos = file.LastIndexOf('\\' + projectName + '\\', StringComparison.Ordinal);
        string path = file[(pos + projectName.Length + 2)..];
        return $"{path}:{line} [{member}]: {_index}->{inn} {lineText}";
    }
}
