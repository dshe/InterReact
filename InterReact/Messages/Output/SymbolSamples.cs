﻿using System.Collections.Generic;

namespace InterReact
{
    public sealed class SymbolSamples : IHasRequestId // output
    {
        public int RequestId { get; }
        public List<ContractDescription> Descriptions { get; } = new List<ContractDescription>();
        internal SymbolSamples(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Descriptions.Add(new ContractDescription(c));
        }
    }
}
