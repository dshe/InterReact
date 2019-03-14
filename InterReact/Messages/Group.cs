using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class DisplayGroupUpdate : IHasRequestId
    {
        public int RequestId { get; }
        public string ContractInfo { get; }
        internal DisplayGroupUpdate(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            ContractInfo = c.ReadString();
        }
    }

    public sealed class DisplayGroups : IHasRequestId
    {
        public int RequestId { get; }
        public string Groups { get; }
        internal DisplayGroups(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            Groups = c.ReadString();
        }
    }

}
