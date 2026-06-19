namespace InterReact;

[Message]
public sealed record ScannerData : IHasRequestId
{
    public int RequestId { get; init; }
    public IList<ScannerDataItem> Items { get; } = [];
    internal ScannerData() { }
    internal ScannerData(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(new ScannerDataItem(r));
    }
}

[Message]
public sealed record ScannerDataItem
{
    public int Rank { get; init; }
    public ContractDetails ContractDetails { get; }
    public string Distance { get; init; } = "";
    public string Benchmark { get; init; } = "";
    public string Projection { get; init; } = "";
    public string ComboLegs { get; init; } = "";

    internal ScannerDataItem() => ContractDetails = new();
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
