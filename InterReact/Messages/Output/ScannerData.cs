using System.Collections.Generic;

namespace InterReact
{
    public interface IScannerData : IHasRequestId { }

    public sealed class ScannerData : IScannerData
    {
        public int RequestId { get; }
        public List<ScannerDataItem> Items { get; } = new List<ScannerDataItem>();
        internal ScannerData(ResponseReader c)
        {
            c.RequireVersion(3);
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Items.Add(new ScannerDataItem(c));
        }
    }

    public sealed class ScannerDataItem
    {
        public int Rank { get; }
        public ContractDetails ContractData { get; }
        public string Distance { get; }
        public string Benchmark { get; }
        public string Projection { get; }
        public string ComboLegs { get; }
        internal ScannerDataItem(ResponseReader c)
        {
            Rank = c.ReadInt();
            ContractData = new ContractDetails(c, ContractDetailsType.ScannerContractData);
            Distance = c.ReadString();
            Benchmark = c.ReadString();
            Projection = c.ReadString();
            ComboLegs = c.ReadString();
        }
    }

}
