using InterReact.Core;
using InterReact.Interfaces;

#nullable enable

namespace InterReact.Messages
{
    /// <summary>
    /// Delta-Neutral Underlying Component.
    /// </summary>
    public sealed class DeltaNeutralContract : IHasRequestId // output
    {
        public int RequestId { get; }

        public int ContractId { get; }

        /// <summary>
        /// The underlying stock or future delta.
        /// </summary>
        public double Delta { get; }

        /// <summary>
        /// The price of the underlying.
        /// </summary>
        public double Price { get; }
        internal DeltaNeutralContract(ResponseComposer c, bool independent)
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
