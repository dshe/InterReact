namespace InterReact
{
    public sealed class AccountSummary : IHasRequestId
    {
        public int RequestId { get; }
        public string Account { get; }
        public string Tag { get; }
        public string Currency { get; }
        public string Value { get; }

        internal AccountSummary(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Account = c.ReadString();
            Tag = c.ReadString();
            Value = c.ReadString();
            Currency = c.ReadString();
        }
    }

    public sealed class AccountSummaryEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal AccountSummaryEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }
    }
}
