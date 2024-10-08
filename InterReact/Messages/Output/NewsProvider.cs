﻿namespace InterReact;

public sealed class NewsProviders
{
    public IList<NewsProvider> Providers { get; }

    internal NewsProviders(ResponseReader r)
    {
        int n = r.ReadInt();
        Providers = new List<NewsProvider>(n);
        for (int i = 0; i < n; i++)
            Providers.Add(new NewsProvider(r));
    }
}

public sealed class NewsProvider
{
    public string Code { get; }
    public string Name { get; }

    internal NewsProvider(ResponseReader r)
    {
        Code = r.ReadString();
        Name = r.ReadString();
    }
}
