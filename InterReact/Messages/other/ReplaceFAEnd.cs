namespace InterReact;

[Message]
public sealed record ReplaceFAEnd : IHasRequestId
{
    public int RequestId { get; init; }
    public string Text { get; init; } = "";
    internal ReplaceFAEnd() { }
    internal ReplaceFAEnd(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Text = r.ReadString();
    }
}
