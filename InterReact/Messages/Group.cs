using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class DisplayGroupUpdate : IHasRequestId
    {
        public int RequestId { get; }
        public string ContractInfo { get; }
        internal DisplayGroupUpdate(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            ContractInfo = c.ReadString();
        }
    }

    public sealed class DisplayGroups : IHasRequestId
    {
        public int RequestId { get; }
        public string Groups { get; }
        internal DisplayGroups(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Groups = c.ReadString();
        }
    }

}
