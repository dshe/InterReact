using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Delta-Neutral Underlying Component.
    /// </summary>
    public sealed class UnderComp : IHasRequestId // output
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
        internal UnderComp(ResponseReader c, bool isContract)
        {
            if (!isContract)
            {
                c.IgnoreVersion();
                RequestId = c.Read<int>();
            }
            ContractId = c.Read<int>();
            Delta = c.Read<double>();
            Price = c.Read<double>();
        }
    }
}
