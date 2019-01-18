using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class TickReqParams : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public double MinTick { get; internal set; }
        public string BboExchange { get; internal set; }
        public int SnapshotPermissions { get; internal set; }
    }
}
