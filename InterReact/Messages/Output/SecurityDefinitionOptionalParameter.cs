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
        internal SecurityDefinitionOptionParameter(ResponseReader r)
        {
            RequestId = r.ReadInt();
            Exchange = r.ReadString();
            UnderlyingContractId = r.ReadInt();
            TradingClass = r.ReadString();
            Multiplier = r.ReadString();
            r.AddStringsToList(Expirations);
            r.AddStringsToList(Strikes);
        }
    }

    public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal SecurityDefinitionOptionParameterEnd(ResponseReader r)
        {
            RequestId = r.ReadInt();
        }
    }
}
