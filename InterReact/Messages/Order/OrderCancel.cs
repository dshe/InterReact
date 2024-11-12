namespace InterReact;

public sealed class OrderCancel // input
{
    //public static readonly OrderCancel Default = new();

    /**
     * @brief This is a regulatory attribute that applies to all US Commodity (Futures) Exchanges, provided to allow client to comply with CFTC Tag 50 Rules
     */
    public string ExtOperator { get; init; } = "";

    /**
     * @brief Used by brokers and advisors when manually entering, modifying or cancelling orders at the direction of a client.
     * <i>Only used when allocating orders to specific groups or accounts. Excluding "All" group.</i>
     */
    public string ManualOrderCancelTime { get; init; } = "";

    /**
     * @brief External User Id
     */
    public string ExternalUserId { get; init; } = "";

    /**
     * @brief Manual Order Indicator
     */
    public int ManualOrderIndicator { get; init; } = int.MaxValue;
}
