namespace InterReact
{
    public enum MarketDataType
    {
        // Live data is streamed back in real time. Market data subscriptions are required to receive live market data.
        // Default
        Realtime = 1,

        // Market data is the last data recorded at market close.
        Frozen = 2,

        // Market data 15-20 minutes behind real-time.
        // Automatically use delayed data if user does not have a real-time subscription.
        // Ignored if real-time data is available.
        Delayed = 3,

        DelayedFrozen = 4
    }
}
