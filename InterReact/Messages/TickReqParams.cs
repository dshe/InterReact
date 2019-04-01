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
        internal TickReqParams(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            MinTick = c.ReadDouble();
            BboExchange = c.ReadString();
            SnapshotPermissions = c.ReadInt();
        }
    };
}
