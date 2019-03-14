using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    // output
    public sealed class SecurityDefinitionOptionParameter : IHasRequestId
    {
        public int RequestId { get; }
        public string Exchange { get; }
        public int UnderlyingContractId { get; }
        public string TradingClass { get; }
        public string Multiplier { get; }
        public IReadOnlyList<string> Expirations { get; }
        public IReadOnlyList<string> Strikes { get; }
        internal SecurityDefinitionOptionParameter(ResponseReader c)
        {
            RequestId = c.Read<int>();
            Exchange = c.ReadString();
            UnderlyingContractId = c.Read<int>();
            TradingClass = c.ReadString();
            Multiplier = c.ReadString();
            Expirations = GetStrings(c.Read<int>());
            Strikes = GetStrings(c.Read<int>());
            List<string> GetStrings(int n) => Enumerable.Repeat(c.ReadString(), n).ToList();
        }
    }

    public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal SecurityDefinitionOptionParameterEnd(ResponseReader c)
        {
            RequestId = c.Read<int>();
        }
    }
}
