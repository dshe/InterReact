using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class AccountUpdateMulti : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Account { get; internal set; }
        public string ModelCode { get; internal set; }
        public string Key { get; internal set; }
        public string Value { get; internal set; }
        public string Currency { get; internal set; }
    }

    public sealed class AccountUpdateMultiEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }


}
