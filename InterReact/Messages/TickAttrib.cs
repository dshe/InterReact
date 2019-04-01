using InterReact.Core;
using InterReact.Enums;
using InterReact.Utility;

namespace InterReact.Messages
{
    public sealed class TickAttrib
    {
        /**
         * @brief Used with tickPrice callback from reqMktData. Specifies whether the price tick is available for automatic execution (1) or not (0).
         */
        public bool CanAutoExecute { get; }

        /**
         * @brief Used with tickPrice to indicate if the bid price is lower than the day's lowest value or the ask price is higher than the highest ask 
         */
        public bool PastLimit { get; }

        /**
         * @brief Indicates whether the bid/ask price tick is from pre-open session
         */
        public bool PreOpen { get; }

        /**
		 * @brief Used with tick-by-tick data to indicate if a trade is classified as 'unreportable' (odd lots, combos, derivative trades, etc)
		*/
        public bool Unreported { get; }

        /**
         * @brief Used with real time tick-by-tick. Indicates if bid is lower than day's lowest low. 
         */
        public bool BidPastLow { get; }

        /**
         * @brief Used with real time tick-by-tick. Indicates if ask is higher than day's highest ask. 
         */
        public bool AskPastHigh { get; }

        internal TickAttrib(ResponseComposer? c)
        {
            if (c == null)
                return;

            var value = c.ReadInt();
            CanAutoExecute = value == 1;
            if (c.Config.SupportsServerVersion(ServerVersion.PastLimit))
            {
                var mask = new BitMask(value);
                CanAutoExecute = mask[0];
                PastLimit = mask[1];
                if (c.Config.SupportsServerVersion(ServerVersion.PreOpenBidAsk))
                    PreOpen = mask[2];
            }
        }
    }

    public class TickAttribBidAsk
    {
        /**
         * @brief Used with real time tick-by-tick. Indicates if bid is lower than day's lowest low. 
         */
        public bool BidPastLow { get; }

        /**
         * @brief Used with real time tick-by-tick. Indicates if ask is higher than day's highest ask. 
         */
        public bool AskPastHigh { get; }

        public TickAttribBidAsk(int value)
        {
            var mask = new BitMask(value);
            BidPastLow = mask[0];
            AskPastHigh = mask[1];
        }
    }

    public class TickAttribLast
    {
        /**
         * @brief Not currently used with trade data; only applies to bid/ask data. 
         */
        public bool PastLimit { get; }

        /**
         * @brief Used with tick-by-tick last data or historical ticks last to indicate if a trade is classified as 'unreportable' (odd lots, combos, derivative trades, etc)
         */
        public bool Unreported { get; }

        public TickAttribLast(int value)
        {
            var mask = new BitMask(value);
            PastLimit = mask[0];
            Unreported = mask[1];
        }

    }
}
