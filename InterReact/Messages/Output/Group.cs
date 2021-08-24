namespace InterReact
{
    public sealed class DisplayGroupUpdate : IHasRequestId
    {
        public int RequestId { get; }
        public string ContractInfo { get; } = "";

        internal DisplayGroupUpdate() { }
        internal DisplayGroupUpdate(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            ContractInfo = r.ReadString();
        }
    }

    public sealed class DisplayGroups : IHasRequestId
    {
        public int RequestId { get; }
        public string Groups { get; } = "";

        internal DisplayGroups() { }
        internal DisplayGroups(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            Groups = r.ReadString();
        }
    }

}
