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
        public IReadOnlyList<string> DerivativeSecTypes { get; }
        internal ContractDescription(ResponseReader c)
        {
            Contract = new Contract
            {
                ContractId = c.Read<int>(),
                Symbol = c.ReadString(),
                SecurityType = c.Read<SecurityType>(),
                PrimaryExchange = c.ReadString(),
                Currency = c.ReadString()
            };
            var n = c.Read<int>();
            DerivativeSecTypes = Enumerable.Repeat(c.ReadString(), n).ToList();
        }
    }

    public sealed class ContractDescriptions : IHasRequestId // output
    {
        public int RequestId { get; }
        public IReadOnlyList<ContractDescription> Descriptions { get; }
        internal ContractDescriptions(ResponseReader c)
        {
            RequestId = c.Read<int>();
            var n = c.Read<int>();
            Descriptions = Enumerable.Repeat(new ContractDescription(c), n).ToList();
        }
    }
}
