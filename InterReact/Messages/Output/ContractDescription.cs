using System.Collections.Generic;

namespace InterReact
{
    public sealed class ContractDescription // output
    {
        public Contract Contract { get; }
        public List<string> DerivativeSecTypes { get; } = new List<string>();
        internal ContractDescription(ResponseReader c)
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
    
}
