namespace InterReact
{
    /// <summary>
    /// Delta-Neutral Underlying Component.
    /// </summary>
    public sealed class DeltaNeutralContract : IHasRequestId // output
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
        
        internal DeltaNeutralContract(ResponseReader c, bool independent)
        {
            if (independent)
            {
                c.IgnoreVersion();
                RequestId = c.ReadInt();
            }
            ContractId = c.ReadInt();
            Delta = c.ReadDouble();
            Price = c.ReadDouble();
        }
    }
}
