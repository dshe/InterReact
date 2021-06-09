using System.Collections.Generic;

namespace InterReact
{
    // output
    public sealed class SecurityDefinitionOptionParameter : IHasRequestId
    {
        public int RequestId { get; }
        public string Exchange { get; }
        public int UnderlyingContractId { get; }
        public string TradingClass { get; }
        public string Multiplier { get; }
        public List<string> Expirations { get; } = new List<string>();
        public List<string> Strikes { get; } = new List<string>();
        internal SecurityDefinitionOptionParameter(ResponseReader c)
        {
            RequestId = c.ReadInt();
            Exchange = c.ReadString();
            UnderlyingContractId = c.ReadInt();
            TradingClass = c.ReadString();
            Multiplier = c.ReadString();
            c.AddStringsToList(Expirations);
            c.AddStringsToList(Strikes);
        }
    }

    public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal SecurityDefinitionOptionParameterEnd(ResponseReader c)
        {
            RequestId = c.ReadInt();
        }
    }
}
