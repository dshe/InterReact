﻿using System.Collections.Generic;
using System.Linq;

namespace InterReact
{
    public sealed class ManagedAccounts
    {
        public string Accounts { get; }
        internal ManagedAccounts(ResponseReader c)
        {
            c.IgnoreVersion();
            Accounts = c.ReadString();
        }
    }
}
