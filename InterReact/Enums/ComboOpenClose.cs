namespace InterReact.Enums
{
    public enum ComboOpenClose
    {
        Undefined = -1,

        /// <summary>
        /// Same as the parent security. This value is always used for retail accounts.
        /// </summary>    
        Same = 0,

        /// <summary>
        /// Institutional Accounts Only.
        /// </summary>    
        Open = 1,

        /// <summary>
        /// Institutional Accounts Only.
        /// </summary>
        Close = 2,

        /// <summary>
        /// Institutional Accounts Only.
        /// </summary>
        Unknown = 3

    }
}
