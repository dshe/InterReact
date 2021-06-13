using System.Collections.Generic;

namespace InterReact
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
        public Dictionary<int, KeyValuePair<string, char>> Map { get; } = new Dictionary<int, KeyValuePair<string, char>>();

        internal SmartComponents(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
            {
                int bitNumber = c.ReadInt();
                string exchange = c.ReadString();
                char exchangeLetter = c.ReadChar();
                Map.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }
        }
    }
}
