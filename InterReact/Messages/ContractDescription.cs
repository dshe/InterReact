using InterReact.Interfaces;
using System.Collections.Generic;

namespace InterReact.Messages
{
    public sealed class ContractDescription // output
    {
        public Contract Contract { get; internal set; }
        public IReadOnlyList<string> DerivativeSecTypes { get; internal set; }
    }

    public sealed class ContractDescriptions : IHasRequestId // output
    {
        public int RequestId { get; internal set; }
        public IReadOnlyList<ContractDescription> Descriptions { get; internal set; }
    }



}
