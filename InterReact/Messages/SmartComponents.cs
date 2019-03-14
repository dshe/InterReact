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
        public IReadOnlyDictionary<int, KeyValuePair<string, char>> Map { get; }

        internal SmartComponents(ResponseReader c)
        {
            RequestId = c.Read<int>();
            var n = c.Read<int>();
            var map = new Dictionary<int, KeyValuePair<string, char>>(n);
            for (var i = 0; i < n; i++)
            {
                var bitNumber = c.Read<int>();
                var exchange = c.ReadString();
                var exchangeLetter = c.Read<char>();
                map.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }
            Map = new ReadOnlyDictionary<int, KeyValuePair<string, char>>(map);
        }
    }
}
