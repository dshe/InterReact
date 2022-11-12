using System.Collections.Generic;

namespace InterReact;

public sealed class ScannerData : IHasRequestId
{
    public int RequestId { get; }
    public IList<ScannerDataItem> Items { get; } = new List<ScannerDataItem>();

    internal ScannerData() { }

    internal ScannerData(ResponseReader r)
    {
        r.RequireVersion(3);
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(new ScannerDataItem(r));
    }
}

public sealed class ScannerDataItem
{
    public int Rank { get; }
    public ContractDetails ContractDetails { get; }
    public string Distance { get; } = "";
    public string Benchmark { get; } = "";
    public string Projection { get; } = "";
    public string ComboLegs { get; } = "";

    internal ScannerDataItem() { ContractDetails = new(); }

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
