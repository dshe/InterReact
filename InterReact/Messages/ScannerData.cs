using System.Collections.Generic;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class ScannerData : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public List<ScannerDataItem> Items { get; internal set; }
    }

    public sealed class ScannerDataItem
    {
        public int Rank { get; internal set; }
        public ContractDetails ContractDetails { get; } = new ContractDetails();
        public string Distance { get; internal set; }
        public string Benchmark { get; internal set; }
        public string Projection { get; internal set; }
        public string ComboLegs { get; internal set; }
    }

}
