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
        public IList<ScannerDataItem> Items { get; } = new List<ScannerDataItem>();
        internal ScannerData(ResponseComposer c)
        {
            c.RequireVersion(3);
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Items.Add(new ScannerDataItem(c));
        }
    }

    public sealed class ScannerDataItem
    {
        public int Rank { get;}
        public ContractData ContractData { get; }
        public string Distance { get;}
        public string Benchmark { get;}
        public string Projection { get;}
        public string ComboLegs { get;}
        internal ScannerDataItem(ResponseComposer c)
        {
            Rank = c.ReadInt();
            ContractData = new ContractData(c, ContractDataType.ScannerContractData);
            Distance = c.ReadString();
            Benchmark = c.ReadString();
            Projection = c.ReadString();
            ComboLegs = c.ReadString();
        }
    }

 }
