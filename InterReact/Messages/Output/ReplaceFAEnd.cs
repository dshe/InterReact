namespace InterReact
{
    public sealed class ReplaceFAEnd : IHasRequestId
    {
        public int RequestId { get; }
        public string Text { get; } = "";

        public ReplaceFAEnd() { }

        public ReplaceFAEnd(ResponseReader reader)
        {
            RequestId = reader.ReadInt();
            Text = reader.ReadString();
        }
    }
}