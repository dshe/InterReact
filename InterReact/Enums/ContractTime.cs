namespace InterReact.Enums
{
    public enum ContractTimeStatus
    {
        /// <summary>
        /// No contract details time information is available.
        /// </summary>
        Undefined,

        /// <summary>
        /// The date value was outside the range of data Values found in the contract details.
        /// </summary>
        OutOfRange,

        /// <summary>
        /// Regular trading hours.
        /// </summary>
        Liquid,

        /// <summary>
        /// Outside of regular trading hours.
        /// </summary>
        Trading,

        /// <summary>
        /// Time when there is no trading.
        /// </summary>
        Closed   
    }

}
