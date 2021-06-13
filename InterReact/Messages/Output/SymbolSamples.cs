using System.Collections.Generic;

namespace InterReact
{
    public interface ISymbolSamples : IHasRequestId { }

    public sealed class SymbolSamples : ISymbolSamples // output
    {
        public int RequestId { get; }
        public List<ContractDescription> Descriptions { get; } = new List<ContractDescription>();
        internal SymbolSamples(ResponseReader c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Descriptions.Add(new ContractDescription(c));
        }
    }
}
