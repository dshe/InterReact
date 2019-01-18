using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    // output
    public sealed class SecurityDefinitionOptionParameter : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Exchange { get; internal set; }
        public int UnderlyingContractId { get; internal set; }
        public string TradingClass { get; internal set; }
        public string Multiplier { get; internal set; }
        public IReadOnlyList<string> Expirations { get; internal set; }
        public IReadOnlyList<string> Strikes { get; internal set; }
    }

    public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
