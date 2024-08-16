namespace InterReact;

public sealed class GenericTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public double Value { get; }

    internal GenericTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Value = r.ReadDouble();
    }
};
