namespace InterReact;

public static partial class Extension
{
    internal static TickType UndelayTick(this TickType tickType)
    {
        return tickType switch
        {
            TickType.DelayedBidPrice => TickType.BidPrice,
            TickType.DelayedAskPrice => TickType.AskPrice,
            TickType.DelayedLastPrice => TickType.LastPrice,
            TickType.DelayedBidSize => TickType.BidSize,
            TickType.DelayedAskSize => TickType.AskSize,
            TickType.DelayedLastSize => TickType.LastSize,
            TickType.DelayedHighPrice => TickType.HighPrice,
            TickType.DelayedLowPrice => TickType.LowPrice,
            TickType.DelayedVolume => TickType.Volume,
            TickType.DelayedClosePrice => TickType.ClosePrice,
            TickType.DelayedOpenPrice => TickType.OpenPrice,
            TickType.DelayedBidOptionComputation => TickType.BidOptionComputation,
            TickType.DelayedAskOptionComputation => TickType.AskOptionComputation,
            TickType.DelayedLastOptionComputation => TickType.LastOptionComputation,
            TickType.DelayedModelOptionComputation => TickType.ModelOptionComputation,
            TickType.DelayedLastTimeStamp => TickType.LastTimeStamp,
            _ => tickType
        };
    }
}
