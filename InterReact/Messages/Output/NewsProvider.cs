using System.Collections.Generic;

namespace InterReact
{
    public sealed class NewsProviders
    {
        public List<NewsProvider> Providors = new();

        internal NewsProviders() { }

        internal NewsProviders(ResponseReader r)
        {
            int n = r.ReadInt();
            for (int i = 0; i < n; i++)
                Providors.Add(new NewsProvider(r));
        }
    }

    public sealed class NewsProvider
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

}
