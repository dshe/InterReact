namespace InterReact
{
    public sealed class RerouteMktData : IHasRequestId
    {
        public int RequestId { get; }
        public int ContractId { get; }
        internal RerouteMktData(ResponseReader c)
        {
            RequestId = c.ReadInt();
            ContractId = c.ReadInt();
        }
    }

    public sealed class RerouteMktDepth : IHasRequestId
    {
        public int RequestId { get; }
        public int ContractId { get; }
        public string Exchange { get; }
        internal RerouteMktDepth(ResponseReader c)
        {
            RequestId = c.ReadInt();
            ContractId = c.ReadInt();
            Exchange = c.ReadString();
        }
    }
}
