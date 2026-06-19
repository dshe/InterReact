namespace InterReact;

[Message]
public sealed record GenericTick : TickBase
{
    public double Value { get; init; }
    internal GenericTick() { }
    internal GenericTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Value = r.ReadDouble();
    }
};
