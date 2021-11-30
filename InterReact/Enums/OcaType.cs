namespace InterReact;

/// <summary>
/// Indicates One-Cancels-All orders.
/// </summary>
public enum OcaType
{
    Undefined = 0,

    /// <summary>
    /// Cancel all remaining orders in the block.
    /// </summary>
    CancelWithBlock = 1,

    /// <summary>
    /// Remaining orders are proportionately reduced in size with block
    /// </summary>
    ReduceWithBlock = 2,

    /// <summary>
    /// Remaining orders are proportionately reduced in size with no block
    /// </summary>
    ReduceWithNoBlock = 3
}
