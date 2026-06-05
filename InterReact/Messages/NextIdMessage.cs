namespace InterReact;

[Message]
public sealed record NextIdMessage
{
    public int NextId { get; }

    internal NextIdMessage(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        NextId = r.ReadInt();
        int oldId = r.Options.Id;
        r.Options.Id = Math.Max(NextId - 1, oldId);
        r.Logger.LogTrace("NextOrderId: {NextId}[{Id1}->{Id2}].", NextId, oldId, r.Options.Id);
    }
}
