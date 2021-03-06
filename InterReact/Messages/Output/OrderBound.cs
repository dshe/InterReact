﻿namespace InterReact
{
    // Orderid is a long here, rather than an int like the other OrderId????
    // MODIFIED!
    public sealed class OrderBound : IHasOrderId
    {
        public long OrderBoundId { get; } // was OrderId
        public int ClientId { get; } // was ApiClientId
        public int OrderId { get; } // was ApiIOrderId
        internal OrderBound(ResponseReader c)
        {
            OrderBoundId = c.ReadLong();
            ClientId = c.ReadInt();
            OrderId = c.ReadInt();
        }
    }
}
