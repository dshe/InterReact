namespace InterReact
{
    public sealed class TickReqParams : IHasRequestId
    {
        public int RequestId { get; }
        public double MinTick { get; }
        public string BboExchange { get; }
        public int SnapshotPermissions { get; }
        internal TickReqParams(ResponseReader c)
        {
            RequestId = c.ReadInt();
            MinTick = c.ReadDouble();
            BboExchange = c.ReadString();
            SnapshotPermissions = c.ReadInt();
        }
    };
}
