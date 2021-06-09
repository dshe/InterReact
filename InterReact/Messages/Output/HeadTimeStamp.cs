namespace InterReact
{
    public sealed class HeadTimestamp : IHasRequestId // output
    {
        public int RequestId { get; }
        public string HeadTimeStamp { get; }

        internal HeadTimestamp(ResponseReader c)
        {
            RequestId = c.ReadInt();
            HeadTimeStamp = c.ReadString();
        }
    }
}
