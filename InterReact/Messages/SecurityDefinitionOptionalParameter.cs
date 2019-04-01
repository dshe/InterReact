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
        public IList<string> Expirations { get; } = new List<string>();
        public IList<string> Strikes { get; } = new List<string>();
        internal SecurityDefinitionOptionParameter(ResponseComposer c)
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
        internal SecurityDefinitionOptionParameterEnd(ResponseComposer c)
        {
            RequestId = c.ReadInt();
        }
    }
}
