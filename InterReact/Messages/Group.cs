using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class DisplayGroupUpdate : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string ContractInfo { get; internal set; }
    }

    public sealed class DisplayGroups : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Groups { get; internal set; }
    }

}
