namespace InterReact;

public sealed class SymbolSamples : IHasRequestId
{
    public int RequestId { get; }
    public IList<ContractDescription> Descriptions { get; }

    internal SymbolSamples(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Descriptions = new List<ContractDescription>(n);
        for (int i = 0; i < n; i++)
            Descriptions.Add(new ContractDescription(r));
    }
}
