using InterReact.Core;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class NewsProvider
    {
        public string Code { get; }
        public string Name { get; }
        internal NewsProvider(ResponseReader c)
        {
            Code = c.ReadString();
            Name = c.ReadString();
        }

        internal static IReadOnlyList<NewsProvider> GetAll(ResponseReader c)
        {
            var n = c.Read<int>();
            return Enumerable.Repeat(new NewsProvider(c), n).ToList();
        }

    }

}
