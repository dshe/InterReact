namespace ClientServer;

public static partial class Extension
{
    public static IObservable<ServerRequestMessage> ToServerRequestMessages(this IObservable<string[]> source)
        => source.Select(strings => new ServerRequestMessage(strings));
}

public sealed class ServerRequestMessage(string[] strings)
{
    internal RequestCode RequestCode { get; } = Enum.Parse<RequestCode>(strings[0]);
    public List<string> Strings { get; } = strings.Skip(1).ToList();
}
