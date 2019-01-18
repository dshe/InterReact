namespace InterReact.Enums
{
    public enum AccountSummaryTag
    {
        AccountType,

        NetLiquidation,

        /// <summary>
        /// Total cash including futures profit and loss.
        /// </summary>
        TotalCashValue,

        /// <summary>
        /// For cash accounts, this is the same as TotalCashValue.
        /// </summary>
        SettledCash,

        /// <summary>
        /// Net accrued interest.
        /// </summary>
        AccruedCash,

        /// <summary>
        /// The maximum amount of marginable US stock the account can buy.
        /// </summary>
        BuyingPower,

        /// <summary>
        /// Cash + stocks + bonds + mutual funds.
        /// </summary>
        EquityWithLoanValue,

        PreviousEquityWithLoanValue,

        /// <summary>
        /// The sum of the absolute value of all stock and equity option positions.
        /// </summary>
        GrossPositionValue,
        RegTEquity,
        RegTMargin,

        /// <summary>
        /// SpecialMemorandumAccount
        ///</summary>
        SMA,

        InitMarginReq,
        MaintMarginReq,
        AvailableFunds,
        ExcessLiquidity,

        /// <summary>
        /// Excess liquidity as a percentage of net liquidation value.
        /// </summary>
        Cushion,

        FullInitMarginReq,
        FullMaintMarginReq,
        FullAvailableFunds,
        FullExcessLiquidity,

        /// <summary>
        /// Time when look-ahead Values take effect.
        /// </summary>
        LookAheadNextChange,
        LookAheadInitMarginReq,
        LookAheadMaintMarginReq,
        LookAheadAvailableFunds,
        LookAheadExcessLiquidity,

        /// <summary>
        /// A measure of how close the account is to liquidation.
        /// </summary>
        HighestSeverity,

        /// <summary>
        /// The Number of Open/Close trades a user could put on before "pattern day trading" is detected.
        /// A value of "-1" means that the user can put on unlimited day trades.
        /// </summary>
        DayTradesRemaining,

        /// <summary>
        /// GrossPositionValue / NetLiquidation.
        /// </summary>
        Leverage
    }
}
