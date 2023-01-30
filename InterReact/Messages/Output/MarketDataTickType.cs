namespace InterReact;

/// <summary>
/// This message indicates a change in MarketDataType. 
/// Tws sends this message to announce that market data has been switched between frozen and real-time.
/// </summary>
public sealed class MarketDataTickType : IHasRequestId
{
    public int RequestId { get; }
    public MarketDataType Type { get; }
 
    internal MarketDataTickType(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Type = r.ReadEnum<MarketDataType>();
    }
}
