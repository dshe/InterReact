using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    // output
    /// <summary>
    /// A response to RequestSmartComponents.
    /// The tick types 'bidExch', 'askExch', 'lastExch' are used to identify the source of a quote.
    /// This result provides a map relating single letters to exchange names.
    /// </summary>
    public sealed class SmartComponents : IHasRequestId
    {
        public int RequestId { get; }
        public IDictionary<int, KeyValuePair<string, char>> Map { get; } = new Dictionary<int, KeyValuePair<string, char>>();

        internal SmartComponents(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (var i = 0; i < n; i++)
            {
                var bitNumber = c.ReadInt();
                var exchange = c.ReadString();
                var exchangeLetter = c.ReadChar();
                Map.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }
        }
    }
}
