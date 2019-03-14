using InterReact.Core;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class ManagedAccounts
    {
        public IReadOnlyList<string> Accounts { get; }
        internal ManagedAccounts(ResponseReader c)
        {
            c.IgnoreVersion();
            Accounts = c.ReadString().Split(',').OrderBy(name => name).ToList();
        }
    }
}
