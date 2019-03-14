using System.Collections.Generic;
using System.Linq;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class ScannerData : IHasRequestId
    {
        public int RequestId { get; }
        public IReadOnlyList<ScannerDataItem> Items { get; } = new List<ScannerDataItem>();
        internal ScannerData(ResponseReader c)
        {
            c.RequireVersion(3);
            RequestId = c.Read<int>();
            var n = c.Read<int>();
            Items = Enumerable.Repeat(new ScannerDataItem(c), n).ToList();
        }
    }

    public sealed class ScannerDataItem
    {
        public int Rank { get;}
        public ContractDetails ContractDetails { get; }
        public string Distance { get;}
        public string Benchmark { get;}
        public string Projection { get;}
        public string ComboLegs { get;}
        internal ScannerDataItem(ResponseReader c)
        {
            Rank = c.Read<int>();
            ContractDetails = new ContractDetails(c, ContractDetailsType.ScannerContractDetails);
            Distance = c.ReadString();
            Benchmark = c.ReadString();
            Projection = c.ReadString();
            ComboLegs = c.ReadString();
        }
    }

 }
