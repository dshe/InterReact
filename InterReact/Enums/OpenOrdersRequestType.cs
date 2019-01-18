namespace InterReact.Enums
{
    public enum OpenOrdersRequestType
    {
        /// <summary>
        /// Open orders submitted with the API using the current clientId.
        /// </summary>
        OpenOrders,

        /// <summary>
        /// All open orders submitted with the API using any clientId.
        /// </summary>
        AllOpenOrders,

        /// <summary>
        /// Open orders submitted manually with TWS.
        /// </summary>
        AutoOpenOrders,

        /// <summary>
        /// Open orders submitted manually with TWS.
        /// If ClientId =  0, such orders will be assigned (bound to) an API orderId and may be modified.
        /// </summary>
        AutoOpenOrdersWithBind
    }

}
