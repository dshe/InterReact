namespace InterReact;

public sealed class ReplaceFAEnd : IHasRequestId
{
    public int RequestId { get; }
    public string Text { get; } = "";

    public ReplaceFAEnd() { }

    internal ReplaceFAEnd(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Text = reader.ReadString();
    }
}
