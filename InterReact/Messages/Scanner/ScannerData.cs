namespace InterReact;

[Message]
public sealed record ScannerData : IHasRequestId
{
    public int RequestId { get; }
    public IList<ScannerDataItem> Items { get; }
    internal ScannerData() => Items = [];
    internal ScannerData(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        Items = new List<ScannerDataItem>(n);
        for (int i = 0; i < n; i++)
            Items.Add(new ScannerDataItem(r));
    }
}

[Message]
public sealed record ScannerDataItem
{
    public int Rank { get; }
    public ContractDetails ContractDetails { get; }
    public string Distance { get; }
    public string Benchmark { get; }
    public string Projection { get; }
    public string ComboLegs { get; }

    internal ScannerDataItem(ResponseReader r)
    {
        Rank = r.ReadInt();
        ContractDetails = new ContractDetails(r, ContractDetailsType.ScannerContractType);
        Distance = r.ReadString();
        Benchmark = r.ReadString();
        Projection = r.ReadString();
        ComboLegs = r.ReadString();
    }
}
