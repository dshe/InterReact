using InterReact.Enums;
using InterReact.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterReact.Utility
{
    public static class TickUtility
    {
        internal static TickType GetTickTypeSize(TickType tickType) => tickType switch
        {
            TickType.BidPrice => TickType.BidSize,
            TickType.AskPrice => TickType.AskSize,
            TickType.LastPrice => TickType.LastSize,
            TickType.DelayedBidPrice => TickType.DelayedBidSize,
            TickType.DelayedAskPrice => TickType.DelayedAskSize,
            TickType.DelayedLastPrice => TickType.DelayedLastSize,
            _ => TickType.Undefined
        };

        internal static TickType UnDelay(TickType delayed)
        {
            return delayed switch
            {
                TickType.DelayedBidPrice => TickType.BidSize,
                TickType.DelayedAskPrice => TickType.AskPrice,
                TickType.DelayedLastPrice => TickType.LastPrice,
                TickType.DelayedBidSize => TickType.BidSize,
                TickType.DelayedAskSize => TickType.AskSize,
                TickType.DelayedLastSize => TickType.LastSize,
                TickType.DelayedHigh => TickType.HighPrice,
                TickType.DelayedLow => TickType.LowPrice,
                TickType.DelayedVolume => TickType.Volume,
                TickType.DelayedClose => TickType.ClosePrice,
                TickType.DelayedOpen => TickType.OpenPrice,
                TickType.DelayedBidOption => TickType.BidOptionComputation,
                TickType.DelayedAskOption => TickType.AskOptionComputation,
                TickType.DelayedLastOption => TickType.LastOptionComputation,
                TickType.DelayedModelOption => TickType.ModelOptionComputation,
                TickType.DelayedLastTimeStamp => TickType.LastTimeStamp,
                _ => delayed
            };
        }

        public static T Undelay<T>(T tick) where T:ITick
        {
            //tick.TickType = UnDelay(tick.TickType);
            return tick;
        }

    }
}
