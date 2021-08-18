using System.Collections.Generic;

namespace InterReact
{
    public sealed class ContractDescription // output
    {
        public Contract Contract { get; }
        public List<string> DerivativeSecTypes { get; } = new List<string>();
        internal ContractDescription(ResponseReader r)
        {
            Contract = new Contract
            {
                ContractId = r.ReadInt(),
                Symbol = r.ReadString(),
                SecurityType = r.ReadStringEnum<SecurityType>(),
                PrimaryExchange = r.ReadString(),
                Currency = r.ReadString()
            };
            r.AddStringsToList(DerivativeSecTypes);
        }
    }
    
}
