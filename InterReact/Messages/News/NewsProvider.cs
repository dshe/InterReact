namespace InterReact;

[Message]
public sealed record NewsProviders
{
    public IList<NewsProvider> Providers { get; } = [];

    internal NewsProviders() { }
    internal NewsProviders(ResponseReader r)
    {
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Providers.Add(new NewsProvider(r));
    }
}

[Message]
public sealed record NewsProvider
{
    public string Code { get; } = "";
    public string Name { get; } = "";
    internal NewsProvider() { }
    internal NewsProvider(ResponseReader r)
    {
        Code = r.ReadString();
        Name = r.ReadString();
    }
}
