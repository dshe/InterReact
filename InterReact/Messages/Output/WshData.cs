namespace InterReact
{
    public sealed class WshMetaData : IHasRequestId
    {
        public int RequestId { get; }
        public string Data { get; } = "";
        public WshMetaData() { }
        public WshMetaData(ResponseReader reader)
        {
            RequestId = reader.ReadInt();
            Data = reader.ReadString();
        }
    }

    public sealed class WshEventData : IHasRequestId
    {
        public int RequestId { get; }
        public string Data { get; } = "";
        public WshEventData() { }
        public WshEventData(ResponseReader reader)
        {
            RequestId = reader.ReadInt();
            Data = reader.ReadString();
        }
    }
}
