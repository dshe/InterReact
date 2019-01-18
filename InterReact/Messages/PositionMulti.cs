using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    public sealed class PositionMulti : IHasRequestId
    {
        public int RequestId { get; internal set; }
        public string Account { get; internal set; }
        public Contract Contract { get; } = new Contract();
        public double Pos { get; internal set; }
        public double AvgCost { get; internal set; }
        public string ModelCode { get; internal set; }
    }

    public sealed class PositionMultiEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
