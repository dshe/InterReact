namespace InterReact;

/// <summary>
/// Delta-Neutral Underlying Component.
/// </summary>
public sealed class DeltaNeutralContract : IHasRequestId
{
    public int RequestId { get; init; }

    public int ContractId { get; init; }

    /// <summary>
    /// The underlying stock or future delta.
    /// </summary>
    public double Delta { get; init; }

    /// <summary>
    /// The price of the underlying.
    /// </summary>
    public double Price { get; init; }

    public DeltaNeutralContract() { }

    internal DeltaNeutralContract(ResponseReader r, bool independent)
    {
        if (independent)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
        }
        ContractId = r.ReadInt();
        Delta = r.ReadDouble();
        Price = r.ReadDouble();
    }
}
