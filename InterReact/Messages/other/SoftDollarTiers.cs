namespace InterReact;

public sealed class SoftDollarTiers : IHasRequestId
{
    public int RequestId { get; }
    public IList<SoftDollarTier> Tiers { get; }
    internal SoftDollarTiers(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Tiers = new List<SoftDollarTier>(n);
        for (int i = 0; i < n; i++)
            Tiers.Add(new SoftDollarTier(r));
    }
}

public sealed class SoftDollarTier
{
    public string Name { get; internal set; } = "";
    public string Value { get; internal set; } = "";
    public string DisplayName { get; internal set; } = "";

    internal SoftDollarTier() { }
    internal SoftDollarTier(ResponseReader r) => Set(r);
    internal void Set(ResponseReader r)
    {
        Name = r.ReadString();
        Value = r.ReadString();
        DisplayName = r.ReadString();
    }
}
