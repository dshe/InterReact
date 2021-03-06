﻿using System.Collections.Generic;

namespace InterReact
{
    public sealed class NewsProviders
    {
        public List<NewsProvider> Providors = new();
        internal NewsProviders(ResponseReader c)
        {
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Providors.Add(new NewsProvider(c));
        }
    }

    public sealed class NewsProvider
    {
        public string Code { get; }
        public string Name { get; }
        internal NewsProvider(ResponseReader c)
        {
            Code = c.ReadString();
            Name = c.ReadString();
        }
    }

}
