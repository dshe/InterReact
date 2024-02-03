namespace ClientServer;

public static partial class Extension
{
    public static IObservable<ServerRequestMessage> ToServerRequestMessages(this IObservable<string[]> source)
        => source.Select(strings => new ServerRequestMessage(strings));
}

public sealed class ServerRequestMessage
{
    internal RequestCode RequestCode { get; }
    public List<string> Strings { get; }
    public ServerRequestMessage(string[] strings)
    {
        RequestCode = Enum.Parse<RequestCode>(strings[0]);
        Strings = strings.Skip(1).ToList();
    }
}
