namespace InterReact
{
    public enum OrderOrigin
    {
        Undefined = -1,

        /// <summary>
        /// Order originated from the customer.
        /// </summary>
        Customer = 0,

        /// <summary>
        /// Order originated from the firm. As a result of a margin call, for example.
        /// </summary>
        Firm = 1
    }
}
