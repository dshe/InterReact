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
        internal AccountUpdateMulti(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Account = c.ReadString();
            ModelCode = c.ReadString();
            Key = c.ReadString();
            Value = c.ReadString();
            Currency = c.ReadString();
        }
    }

    public sealed class AccountUpdateMultiEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
        internal AccountUpdateMultiEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }
    }
}
