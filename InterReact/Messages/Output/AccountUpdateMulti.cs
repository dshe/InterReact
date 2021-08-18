namespace InterReact
{
    public sealed class AccountUpdateMulti : IHasRequestId
    {
        public int RequestId { get; }
        public string Account { get; }
        public string ModelCode { get; }
        public string Key { get; }
        public string Value { get; }
        public string Currency { get; }
        internal AccountUpdateMulti(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            Account = r.ReadString();
            ModelCode = r.ReadString();
            Key = r.ReadString();
            Value = r.ReadString();
            Currency = r.ReadString();
        }
    }

    public sealed class AccountUpdateMultiEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
        internal AccountUpdateMultiEnd(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
        }
    }
}
