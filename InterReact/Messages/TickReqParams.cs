using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class TickReqParams : IHasRequestId
    {
        public int RequestId { get;}
        public double MinTick { get;}
        public string BboExchange { get;}
        public int SnapshotPermissions { get;}
        internal TickReqParams(ResponseReader c)
        {
            RequestId = c.Read<int>();
            MinTick = c.Read<double>();
            BboExchange = c.ReadString();
            SnapshotPermissions = c.Read<int>();
        }
    };
}
