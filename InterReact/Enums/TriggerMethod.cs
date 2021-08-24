namespace InterReact
{
    public enum TriggerMethod
    {
        /// <summary>
        /// Default. The double bid/ask method will be used for orders for OTC stocks and US options. 
        /// All Status orders will used the "last" method.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Orders are triggered based on two consecutive bid or ask prices.
        /// </summary>
        DoubleBidAsk = 1,

        Last = 2,
        DoubleLast = 3,
        BidAsk = 4,
        LastOfBidAsk = 7,
        Midpoint = 8
    }

}