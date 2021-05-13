namespace InterReact
{
    public enum HaltType
    {
        Undefined = -1,

        NotHalted = 0,

        /// <summary>
        /// General halt (trading halt is imposed for purely regulatory reasons) with/without volatility halt.
        /// </summary>
        GeneralHalt = 1,

        /// <summary>
        /// Volatility only halt (trading halt is imposed by the exchange to protect against extreme volatility).
        /// </summary>
        VolatilityHalt = 2
    }
}
