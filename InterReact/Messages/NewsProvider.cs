using InterReact.Core;
using System.Collections.Generic;

namespace InterReact.Messages
{
    public sealed class NewsProviders
    {
        public IList<NewsProvider> Providors = new List<NewsProvider>();
        internal NewsProviders(ResponseComposer c)
        {
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Providors.Add(new NewsProvider(c));
        }
    }

    public sealed class NewsProvider
    {
        public string Code { get; }
        public string Name { get; }
        internal NewsProvider(ResponseComposer c)
        {
            Code = c.ReadString();
            Name = c.ReadString();
        }
    }

}
