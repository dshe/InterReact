namespace InterReact;

[Message]
public sealed record ReplaceFAEnd : IHasRequestId
{
    public int RequestId { get; }
    public string Text { get; }

    internal ReplaceFAEnd(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Text = reader.ReadString();
    }
}
