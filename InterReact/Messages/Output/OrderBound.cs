namespace InterReact
{
    // Orderid is a long here, rather than an int like the other OrderId????
    // MODIFIED!
    public sealed class OrderBound : IHasOrderId
    {
        public long OrderBoundId { get; } // was OrderId
        public int ClientId { get; } // was ApiClientId
        public int OrderId { get; } // was ApiIOrderId
        internal OrderBound(ResponseReader r)
        {
            OrderBoundId = r.ReadLong();
            ClientId = r.ReadInt();
            OrderId = r.ReadInt();
        }
    }
}
