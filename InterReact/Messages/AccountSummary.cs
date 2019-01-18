using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class AccountSummary : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Account { get; internal set; }
        public string Tag { get; internal set; }
        public string Currency { get; internal set; }
        public string Value { get; internal set; }
    }

    public sealed class AccountSummaryEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
