using System.Collections.Generic;

namespace InterReact;

// output
/// <summary>
/// A response to RequestSmartComponents.
/// The tick types 'bidExch', 'askExch', 'lastExch' are used to identify the source of a quote.
/// This result provides a map relating single letters to exchange names.
/// </summary>
public sealed class SmartComponents : IHasRequestId
{
    public int RequestId { get; }
    public Dictionary<int, KeyValuePair<string, char>> Map { get; } = new();

    internal SmartComponents() { }

    internal SmartComponents(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
        {
            int bitNumber = r.ReadInt();
            string exchange = r.ReadString();
            char exchangeLetter = r.ReadChar();
            Map.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
        }
    }
}
