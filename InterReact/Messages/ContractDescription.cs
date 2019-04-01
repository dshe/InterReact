using InterReact.Core;
using InterReact.Interfaces;
using InterReact.StringEnums;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class ContractDescription // output
    {
        public Contract Contract { get; }
        public IList<string> DerivativeSecTypes { get; } = new List<string>();
        internal ContractDescription(ResponseComposer c)
        {
            Contract = new Contract
            {
                ContractId = c.ReadInt(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                PrimaryExchange = c.ReadString(),
                Currency = c.ReadString()
            };
            c.AddStringsToList(DerivativeSecTypes);
        }
    }

    public sealed class SymbolSamples : IHasRequestId // output
    {
        public int RequestId { get; }
        public IList<ContractDescription> Descriptions { get; } = new List<ContractDescription>();
        internal SymbolSamples(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Descriptions.Add(new ContractDescription(c));
        }
    }
}
