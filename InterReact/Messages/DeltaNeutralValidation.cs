using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Delta-Neutral Underlying Component.
    /// </summary>
    public sealed class UnderComp : IHasRequestId // output
    {
        public int RequestId { get; set; }

        public int ContractId { get; set; }

        /// <summary>
        /// The underlying stock or future delta.
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// The price of the underlying.
        /// </summary>
        public double Price { get; set; }
    }
}
