using InterReact.Core;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class ManagedAccounts
    {
        public IList<string> Accounts { get; }
        internal ManagedAccounts(ResponseComposer c)
        {
            c.IgnoreVersion();
            Accounts = c.ReadString().Split(',').OrderBy(name => name).ToList();
        }
    }
}
