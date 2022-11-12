using System.Collections.Generic;

namespace InterReact;

public sealed class SecurityDefinitionOptionParameter : IHasRequestId
{
    public int RequestId { get; }
    public string Exchange { get; } = "";
    public int UnderlyingContractId { get; }
    public string TradingClass { get; } = "";
    public string Multiplier { get; } = "";
    public IList<string> Expirations { get; } = new List<string>();
    public IList<double> Strikes { get; } = new List<double>();

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

public sealed class SecurityDefinitionOptionParameterEnd : IHasRequestId
{
    public int RequestId { get; }
    internal SecurityDefinitionOptionParameterEnd(ResponseReader r)
    {
        RequestId = r.ReadInt();
    }
}
