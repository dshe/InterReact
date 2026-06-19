namespace InterReact;

[Message]
public sealed record SecurityDefinitionOptionParameter : IHasRequestId
{
    public int RequestId { get; init; }
    public string Exchange { get; init; } = "";
    public int UnderlyingContractId { get; init; }
    public string TradingClass { get; init; } = "";
    public string Multiplier { get; init; } = "";
    public IList<string> Expirations { get; } = [];
    public IList<double> Strikes { get; } = [];
    internal SecurityDefinitionOptionParameter() { }
    internal SecurityDefinitionOptionParameter(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Exchange = r.ReadString();
        UnderlyingContractId = r.ReadInt();
        TradingClass = r.ReadString();
        Multiplier = r.ReadString();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Expirations.Add(r.ReadString());
        n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Strikes.Add(r.ReadDouble());
    }
}

[Message]
public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
{
    public int RequestId { get; }
    internal SecurityDefinitionOptionParameterEnd() { }
    internal SecurityDefinitionOptionParameterEnd(ResponseReader r) => RequestId = r.ReadInt();
}
