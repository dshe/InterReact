using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Delta-Neutral Underlying Component.
    /// </summary>
    public sealed class DeltaNeutralContract : IHasRequestId // output
    {
        public int RequestId { get; private set; }

        public int ContractId { get; internal set; }

        /// <summary>
        /// The underlying stock or future delta.
        /// </summary>
        public double Delta { get; internal set; }

        /// <summary>
        /// The price of the underlying.
        /// </summary>
        public double Price { get; internal set; }
        internal DeltaNeutralContract() { } // testing
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
